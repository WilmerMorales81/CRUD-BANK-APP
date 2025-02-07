using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CrudBankApp.Data;
using CrudBankApp.Models;
using CrudBankApp.Models.DTOs;

namespace CrudBankApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly CrudBankAppDbContext _dbContext;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(CrudBankAppDbContext dbContext, ILogger<AccountsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }



        // GET: api/Accounts
        [HttpGet]
public async Task<IActionResult> GetAll()
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        IQueryable<Account> accountsQuery = _dbContext.Accounts
            .Include(a => a.AccountType)
            .Include(a => a.UserProfile)
                .ThenInclude(up => up.IdentityUser);  // Include IdentityUser for email

        // If not admin, filter by user
        if (!isAdmin)
        {
            accountsQuery = accountsQuery.Where(a => a.UserProfile.IdentityUserId == userId);
        }

        var accounts = await accountsQuery.Select(a => new AccountDTO
        {
            Id = a.Id,
            Number = a.Number,
            AccountTypeId = a.AccountTypeId,
            AccountTypeName = a.AccountType.Name,
            Balance = a.Balance,
            MinPay = a.MinPay,
            UserProfileId = a.UserProfileId,
            CreatedAt = a.CreatedAt,
            AccountType = new AccountTypeDTO
            {
                Id = a.AccountType.Id,
                Name = a.AccountType.Name
            },
            UserProfile = new UserProfileDTO
            {
                Id = a.UserProfile.Id,
                FirstName = a.UserProfile.FirstName,
                LastName = a.UserProfile.LastName,
                Email = a.UserProfile.IdentityUser.Email,
                Phone = a.UserProfile.Phone,
                Address = a.UserProfile.Address
            }
        }).ToListAsync();

        return Ok(accounts);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving accounts");
        return StatusCode(500, new { Message = "Error retrieving accounts" });
    }
}

        // GET: api/Accounts/user
        [HttpGet("user")]
        public async Task<IActionResult> GetUserAccounts()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var accounts = await _dbContext.Accounts
                    .Include(a => a.AccountType)
                    .Where(a => a.UserProfile.IdentityUserId == userId)
                    .Select(a => new AccountDTO
                    {
                        Id = a.Id,
                        Number = a.Number,
                        AccountTypeId = a.AccountTypeId,
                        AccountTypeName = a.AccountType.Name,
                        Balance = a.Balance,
                        MinPay = a.MinPay,
                        UserProfileId = a.UserProfileId
                    })
                    .ToListAsync();

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user accounts");
                return StatusCode(500, new { Message = "Error retrieving accounts" });
            }
        }

        

        
        [HttpPost("pay/{id}")]
public async Task<IActionResult> PayAccount(int id, [FromBody] PaymentDTO paymentDto)
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");  // Add admin check

        var account = await _dbContext.Accounts
            .Include(a => a.UserProfile)
            .FirstOrDefaultAsync(a => a.Id == id && 
                (isAdmin || a.UserProfile.IdentityUserId == userId));  // Allow admin access

        if (account == null)
        {
            return NotFound(new { Message = "Account not found or unauthorized" });
        }

        if (paymentDto.Amount <= 0)
        {
            return BadRequest(new { Message = "Payment amount must be greater than zero" });
        }

        if (paymentDto.Amount > account.Balance)
        {
            return BadRequest(new { Message = "Payment amount cannot exceed balance" });
        }

        // Process payment
        account.Balance -= paymentDto.Amount;

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            Message = "Payment processed successfully",
            NewBalance = account.Balance,
            NewMinimumPayment = account.MinPay
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing payment");
        return StatusCode(500, new { Message = "Error processing payment" });
    }
}


        // POST: api/Accounts
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDTO createAccountDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userProfile = await _dbContext.UserProfiles
                    .FirstOrDefaultAsync(up => up.IdentityUserId == userId);

                if (userProfile == null)
                {
                    return NotFound(new { Message = "User profile not found" });
                }

                // Validate account type exists
                var accountType = await _dbContext.AccountTypes
                    .FindAsync(createAccountDto.AccountTypeId);
                if (accountType == null)
                {
                    return BadRequest(new { Message = "Invalid account type" });
                }

                // Validate initial balance
                if (createAccountDto.InitialBalance <= 0)
                {
                    return BadRequest(new { Message = "Initial balance must be greater than zero" });
                }

                // Generate unique account number
                var accountNumber = $"{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8]}";

                var account = new Account
                {
                    Number = accountNumber,
                    AccountTypeId = createAccountDto.AccountTypeId,
                    Balance = createAccountDto.InitialBalance, // This will automatically set MinPay
                    UserProfileId = userProfile.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Accounts.Add(account);
                await _dbContext.SaveChangesAsync();

                var accountDto = new AccountDTO
                {
                    Id = account.Id,
                    Number = account.Number,
                    AccountTypeId = account.AccountTypeId,
                    AccountTypeName = accountType.Name,
                    Balance = account.Balance,
                    MinPay = account.MinPay,
                    UserProfileId = account.UserProfileId
                };

                return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, accountDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account");
                return StatusCode(500, new { Message = "Error creating account" });
            }
        }

        // GET: api/Accounts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var account = await _dbContext.Accounts
                    .Include(a => a.AccountType)
                    .FirstOrDefaultAsync(a => a.Id == id &&
                        a.UserProfile.IdentityUserId == userId);

                if (account == null)
                {
                    return NotFound(new { Message = "Account not found or unauthorized" });
                }

                var accountDto = new AccountDTO
                {
                    Id = account.Id,
                    Number = account.Number,
                    AccountTypeId = account.AccountTypeId,
                    AccountTypeName = account.AccountType.Name,
                    Balance = account.Balance,
                    MinPay = account.MinPay,
                    UserProfileId = account.UserProfileId
                };

                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account");
                return StatusCode(500, new { Message = "Error retrieving account" });
            }
        }

        // PUT: api/Accounts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDTO updateAccountDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var account = await _dbContext.Accounts
                    .Include(a => a.AccountType)
                    .FirstOrDefaultAsync(a => a.Id == id &&
                        a.UserProfile.IdentityUserId == userId);

                if (account == null)
                {
                    return NotFound(new { Message = "Account not found or unauthorized" });
                }

                // Only allow updating certain fields
                if (updateAccountDto.AccountTypeId.HasValue)
                {
                    var newAccountType = await _dbContext.AccountTypes
                        .FindAsync(updateAccountDto.AccountTypeId.Value);
                    if (newAccountType == null)
                    {
                        return BadRequest(new { Message = "Invalid account type" });
                    }
                    account.AccountTypeId = updateAccountDto.AccountTypeId.Value;
                }

                await _dbContext.SaveChangesAsync();

                var accountDto = new AccountDTO
                {
                    Id = account.Id,
                    Number = account.Number,
                    AccountTypeId = account.AccountTypeId,
                    AccountTypeName = account.AccountType.Name,
                    Balance = account.Balance,
                    MinPay = account.MinPay,
                    UserProfileId = account.UserProfileId
                };

                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account");
                return StatusCode(500, new { Message = "Error updating account" });
            }
        }

        // DELETE: api/Accounts/{id}
        [HttpDelete("{id}")]
public async Task<IActionResult> DeleteAccount(int id)
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");  // Add admin check

        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == id && 
                (isAdmin || a.UserProfile.IdentityUserId == userId));  // Allow admin access

        if (account == null)
        {
            return NotFound(new { Message = "Account not found or unauthorized" });
        }

        // Business rule: Cannot delete account with positive balance
        if (account.Balance > 0)
        {
            return BadRequest(new { Message = "Cannot delete account with positive balance" });
        }

        _dbContext.Accounts.Remove(account);
        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Account deleted successfully" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error deleting account");
        return StatusCode(500, new { Message = "Error deleting account" });
    }
}
        // GET: api/accounts/{id}/customer
        [HttpGet("{id}/customer")]
        public async Task<IActionResult> GetAccountCustomer(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                var account = await _dbContext.Accounts
                    .Include(a => a.UserProfile)
                        .ThenInclude(up => up.IdentityUser)
                    .FirstOrDefaultAsync(a => a.Id == id &&
                        (isAdmin || a.UserProfile.IdentityUserId == userId));

                if (account == null)
                {
                    return NotFound(new { Message = "Account not found or unauthorized" });
                }

                var customerDto = new UserProfileDTO
                {
                    Id = account.UserProfile.Id,
                    FirstName = account.UserProfile.FirstName,
                    LastName = account.UserProfile.LastName,
                    Email = account.UserProfile.IdentityUser.Email,
                    Phone = account.UserProfile.Phone,
                    Address = account.UserProfile.Address
                };

                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account customer");
                return StatusCode(500, new { Message = "Error retrieving customer details" });
            }
        }

        [HttpPut("{id}/customer")]
public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserProfileDTO updateProfileDto)
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        // First verify the account exists and user has access
        var account = await _dbContext.Accounts
            .Include(a => a.UserProfile)
                .ThenInclude(up => up.IdentityUser)
            .FirstOrDefaultAsync(a => a.Id == id && 
                (isAdmin || a.UserProfile.IdentityUserId == userId));

        if (account == null)
        {
            return NotFound(new { Message = "Account not found or unauthorized" });
        }

        // Update the profile information
        account.UserProfile.FirstName = updateProfileDto.FirstName;
        account.UserProfile.LastName = updateProfileDto.LastName;
        account.UserProfile.Address = updateProfileDto.Address;
        account.UserProfile.Phone = updateProfileDto.Phone;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error while updating profile");
            return StatusCode(500, new { Message = "Database error while updating profile" });
        }

        // Return updated profile
        var profileDto = new UserProfileDTO
        {
            Id = account.UserProfile.Id,
            FirstName = account.UserProfile.FirstName,
            LastName = account.UserProfile.LastName,
            Email = account.UserProfile.IdentityUser.Email,
            Phone = account.UserProfile.Phone,
            Address = account.UserProfile.Address
        };

        return Ok(profileDto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating user profile");
        return StatusCode(500, new { Message = "Error updating profile information" });
    }
}
    }
}