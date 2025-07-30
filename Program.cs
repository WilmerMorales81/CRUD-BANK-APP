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


var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure DbContext
Console.WriteLine("=== RAILWAY DEBUG ===");
Console.WriteLine($"CRUD_BANK_CONN: '{Environment.GetEnvironmentVariable("CRUD_BANK_CONN")}'");
Console.WriteLine($"ConnectionStrings__CrudBankAppDbConnectionString: '{Environment.GetEnvironmentVariable("ConnectionStrings__CrudBankAppDbConnectionString")}'");
Console.WriteLine("All environment variables:");
foreach (var kv in Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>())
{
    Console.WriteLine($"  {kv.Key} = {kv.Value}");
}
Console.WriteLine("=== END DEBUG ===");

var connectionString = Environment.GetEnvironmentVariable("CRUD_BANK_CONN") 
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__CrudBankAppDbConnectionString")
    ?? builder.Configuration.GetConnectionString("CrudBankAppDbConnectionString");

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
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
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
                "http://localhost:5173", // Your Vite frontend URL (local)
                "https://crud-bank-app.vercel.app", // Production frontend URL
                "https://your-frontend-domain.vercel.app" // Replace with your actual Vercel domain
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Authorization");
    });
});

var app = builder.Build();

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
    var adminEmail = configuration["AdminEmail"];
    var adminPassword = configuration["AdminPassword"];

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