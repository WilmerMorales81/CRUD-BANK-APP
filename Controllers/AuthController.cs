using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CrudBankApp.Models.DTOs;
using CrudBankApp.Models;
using CrudBankApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CrudBankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly CrudBankAppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            CrudBankAppDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _dbContext = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Login attempt for email: {request.Email}");

                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    Console.WriteLine("[DEBUG] Login failed: Empty email or password");
                    return BadRequest(new { Message = "Email and password are required." });
                }

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    Console.WriteLine($"[DEBUG] Login failed: User not found for email {request.Email}");
                    return Unauthorized(new { Message = "Invalid email or password." });
                }

                Console.WriteLine($"[DEBUG] User found with ID: {user.Id}");

                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    Console.WriteLine($"[DEBUG] Login failed: Invalid password for user {request.Email}");
                    return Unauthorized(new { Message = "Invalid email or password." });
                }

                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"[DEBUG] User roles: {string.Join(", ", roles)}");

                var token = GenerateJwtToken(user, roles);
                Console.WriteLine("[DEBUG] Token generated successfully");

                return Ok(new
                {
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        Roles = roles
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Login exception: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { Message = "An error occurred during login." });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // For JWT tokens, logout is handled client-side by removing the token
            // This endpoint just returns success to prevent 404 errors
            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"\n[DEBUG ME] Looking for user with ID: {userId}");

                // Get user profile with accounts and account types
                var profile = await _dbContext.UserProfiles
                    .Include(up => up.Accounts)
                        .ThenInclude(a => a.AccountType)
                    .FirstOrDefaultAsync(up => up.IdentityUserId == userId);

                // Debug: Get all profiles to see what exists
                var allProfiles = await _dbContext.UserProfiles.ToListAsync();
                Console.WriteLine("[DEBUG ME] All profiles in database:");
                foreach (var p in allProfiles)
                {
                    Console.WriteLine($"Profile ID: {p.Id}, IdentityUserId: {p.IdentityUserId}");
                }

                if (profile == null)
                {
                    return NotFound(new
                    {
                        Message = "Profile not found",
                        RequestedUserId = userId,
                        AvailableProfiles = allProfiles.Select(p => new { p.Id, p.IdentityUserId })
                    });
                }

                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser == null)
                {
                    return NotFound(new { Message = "Identity user not found." });
                }

                var roles = await _userManager.GetRolesAsync(identityUser);

                var userDto = new UserProfileDTO
                {
                    Id = profile.Id,
                    IdentityUserId = profile.IdentityUserId,
                    UserName = identityUser.UserName,
                    Email = identityUser.Email,
                    Roles = roles.ToList(),
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Address = profile.Address,
                    Phone = profile.Phone,
                    Accounts = profile.Accounts.Select(a => new AccountDTO
                    {
                        Id = a.Id,
                        Number = a.Number,
                        AccountTypeId = a.AccountTypeId,
                        Balance = a.Balance,
                        MinPay = a.MinPay,
                        UserProfileId = a.UserProfileId,
                        AccountTypeName = a.AccountType.Name,
                        AccountType = new AccountTypeDTO
                        {
                            Id = a.AccountType.Id,
                            Name = a.AccountType.Name
                        }
                    }).ToList()
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR ME] Exception: {ex.Message}");
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Registration attempt for email: {request.Email}");

                // Validate request
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid registration data", Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "User with this email already exists." });
                }

                // Create Identity User
                var user = new IdentityUser
                {
                    UserName = request.Email,  // Using email as username
                    Email = request.Email,
                    EmailConfirmed = true  // Auto-confirm for development
                };

                // Create user with password
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new { Message = "Failed to create user", Errors = result.Errors });
                }

                // Assign Customer role
                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                if (!roleResult.Succeeded)
                {
                    // If role assignment fails, delete the user and return error
                    await _userManager.DeleteAsync(user);
                    return StatusCode(500, new { Message = "Failed to assign role", Errors = roleResult.Errors });
                }

                // Create user profile
                var userProfile = new UserProfile
                {
                    IdentityUserId = user.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = request.Address,
                    Phone = request.Phone
                };

                _dbContext.UserProfiles.Add(userProfile);
                await _dbContext.SaveChangesAsync();

                // Generate token for immediate login
                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);

                return Ok(new
                {
                    Message = "Registration successful",
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.Email,
                        userProfile.FirstName,
                        userProfile.LastName,
                        Roles = roles
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Registration exception: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { Message = "An error occurred during registration." });
            }
        }

        [HttpGet("customers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _dbContext.UserProfiles
                .Include(up => up.IdentityUser)
                .Where(up => _userManager.GetRolesAsync(up.IdentityUser).Result.Contains("Customer"))
                .Select(up => new UserProfileDTO
                {
                    Id = up.Id,
                    IdentityUserId = up.IdentityUserId,
                    Email = up.IdentityUser.Email,
                    FirstName = up.FirstName,
                    LastName = up.LastName,
                    Address = up.Address,
                    Phone = up.Phone
                })
                .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("customers/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _dbContext.UserProfiles
                .Include(up => up.IdentityUser)
                .FirstOrDefaultAsync(up => up.Id == id);

            if (customer == null)
            {
                return NotFound(new { Message = "Customer not found" });
            }

            var roles = await _userManager.GetRolesAsync(customer.IdentityUser);
            if (!roles.Contains("Customer"))
            {
                return NotFound(new { Message = "User is not a customer" });
            }

            var customerDto = new UserProfileDTO
            {
                Id = customer.Id,
                IdentityUserId = customer.IdentityUserId,
                Email = customer.IdentityUser.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                Phone = customer.Phone
            };

            return Ok(customerDto);
        }

        [HttpGet("debug/users")]
        public async Task<IActionResult> DebugUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDetails = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var profile = await _dbContext.UserProfiles
                    .FirstOrDefaultAsync(up => up.IdentityUserId == user.Id);

                userDetails.Add(new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    Roles = roles,
                    Profile = profile != null ? new
                    {
                        profile.Id,
                        profile.FirstName,
                        profile.LastName
                    } : null
                });
            }

            return Ok(userDetails);
        }

        private string GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Get JWT configuration with fallbacks (same as Program.cs)
            var jwtKey = _configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("Jwt__Key") ?? "ThisIsMySecretKey123!@#$%ThisIsMySecretKey123!@#$%";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("Jwt__Issuer") ?? "https://crud-bank-app-production.up.railway.app";
            var jwtAudience = _configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("Jwt__Audience") ?? "https://crud-bank-app.vercel.app";
            var jwtExpirationHours = _configuration["Jwt:ExpirationHours"] ?? "24";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(Convert.ToDouble(jwtExpirationHours));

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}