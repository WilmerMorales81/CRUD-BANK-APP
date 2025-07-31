using System.Security.Claims;
using System.Text;
using CrudBankApp.Data;
using CrudBankApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure DbContext
Console.WriteLine("=== RAILWAY DEBUG START ===");
Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
Console.WriteLine($"Environment variables count: {Environment.GetEnvironmentVariables().Count}");

var crudBankConn = Environment.GetEnvironmentVariable("CRUD_BANK_CONN");
var connectionStringsConn = Environment.GetEnvironmentVariable("ConnectionStrings__CrudBankAppDbConnectionString");
var configConn = builder.Configuration.GetConnectionString("CrudBankAppDbConnectionString");

Console.WriteLine($"CRUD_BANK_CONN: '{crudBankConn}'");
Console.WriteLine($"ConnectionStrings__CrudBankAppDbConnectionString: '{connectionStringsConn}'");
Console.WriteLine($"Config connection string: '{configConn}'");

// Try to get connection string from multiple sources
var connectionString = crudBankConn ?? connectionStringsConn ?? configConn;

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("=== NO CONNECTION STRING FOUND ===");
    Console.WriteLine("Available environment variables:");
    foreach (var kv in Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>())
    {
        Console.WriteLine($"  {kv.Key} = {kv.Value}");
    }
    Console.WriteLine("=== END DEBUG ===");
    
    // For Railway, let's try a hardcoded connection string as fallback
    connectionString = "Host=ep-young-resonance-aeqgjf89-pooler.c-2.us-east-2.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_uXl0QsVY7iIW;SSL Mode=Require;Trust Server Certificate=true";
    Console.WriteLine("Using fallback connection string for Railway");
}

Console.WriteLine($"Final connection string: {connectionString}");
Console.WriteLine("=== RAILWAY DEBUG END ===");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is not configured. Please set the CRUD_BANK_CONN or ConnectionStrings__CrudBankAppDbConnectionString environment variable.");
}

builder.Services.AddDbContext<CrudBankAppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<CrudBankAppDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
// Update the JWT Authentication configuration section
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    
    // Get JWT configuration with fallbacks
    var jwtKey = builder.Configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("Jwt__Key") ?? "ThisIsMySecretKey123!@#$%ThisIsMySecretKey123!@#$%";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("Jwt__Issuer") ?? "https://crud-bank-app-production.up.railway.app";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("Jwt__Audience") ?? "https://crud-bank-app.vercel.app";
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CrudBankApp API", Version = "v1" });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Replace the existing CORS configuration with this:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(
                "http://localhost:5173", // Local development
                "https://crud-bank-app.vercel.app" // Production Vercel domain
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Authorization");
    });
});

var app = builder.Build();

// Add error handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            Console.WriteLine($"Error: {error.Error.Message}");
            Console.WriteLine($"StackTrace: {error.Error.StackTrace}");
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "An error occurred",
                details = error.Error.Message
            }));
        }
    });
});

// Create roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = services.GetRequiredService<CrudBankAppDbContext>();
    var configuration = services.GetRequiredService<IConfiguration>();

    // Create roles first
    string[] roles = { "Admin", "Customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Get or create admin user
    var adminEmail = configuration["AdminEmail"] ?? Environment.GetEnvironmentVariable("AdminEmail") ?? "admin@crudbank.com";
    var adminPassword = configuration["AdminPassword"] ?? Environment.GetEnvironmentVariable("AdminPassword") ?? "Admin123!";

    Console.WriteLine($"Setting up admin user with email: {adminEmail}");

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = "Administrator",
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors)}");
        }
    }

    // Ensure admin role
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
        if (!roleResult.Succeeded)
        {
            throw new Exception($"Failed to assign admin role: {string.Join(", ", roleResult.Errors)}");
        }
    }

    // Ensure admin profile exists
    var adminProfile = await dbContext.UserProfiles
        .FirstOrDefaultAsync(up => up.IdentityUserId == adminUser.Id);

    if (adminProfile == null)
    {
        adminProfile = new UserProfile
        {
            IdentityUserId = adminUser.Id,
            FirstName = "Admin",
            LastName = "User",
            Address = "Admin Address",
            Phone = "0000000000"
        };

        dbContext.UserProfiles.Add(adminProfile);
        await dbContext.SaveChangesAsync();
    }
}




// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Add endpoint mapping after authorization
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

     // ---------- Health check ----------
    endpoints.MapGet("/health", async context =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"status\":\"Healthy\"}");
    });
});

app.Run();