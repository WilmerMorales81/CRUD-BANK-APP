using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudBankApp.Data;
using CrudBankApp.Models.DTOs;

namespace CrudBankApp.Controllers;

[ApiController]
[Route("api/userprofile")]
//[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly CrudBankAppDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public UserProfileController(CrudBankAppDbContext context, UserManager<IdentityUser> userManager)
    {
        _dbContext = context;
        _userManager = userManager;
    }

    // ✅ Obtener todos los perfiles de usuario
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userProfiles = await _dbContext.UserProfiles
            .Include(up => up.IdentityUser)  // Asegurar la relación con IdentityUser
            .Include(up => up.Accounts)
                .ThenInclude(a => a.AccountType)
            .ToListAsync();

        var userProfileDTOs = new List<UserProfileDTO>();

        foreach (var up in userProfiles)
        {
            var identityUser = await _userManager.FindByIdAsync(up.IdentityUserId);
            var roles = await _userManager.GetRolesAsync(identityUser);

            userProfileDTOs.Add(new UserProfileDTO
            {
                Id = up.Id,
                IdentityUserId = up.IdentityUserId,
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                Roles = roles.ToList(),
                FirstName = up.FirstName,
                LastName = up.LastName,
                Address = up.Address,
                Phone = up.Phone,
                Accounts = up.Accounts.Select(a => new AccountDTO
                {
                    Id = a.Id,
                    Number = a.Number,
                    AccountTypeId = a.AccountTypeId,
                    Balance = a.Balance,
                    MinPay = a.MinPay,
                    AccountType = new AccountTypeDTO
                    {
                        Id = a.AccountType.Id,
                        Name = a.AccountType.Name
                    }
                }).ToList()
            });
        }

        return Ok(userProfileDTOs);
    }

    // ✅ Obtener un perfil de usuario por ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var profile = await _dbContext.UserProfiles
            .Include(up => up.IdentityUser)
            .Include(up => up.Accounts)
                .ThenInclude(a => a.AccountType)
            .FirstOrDefaultAsync(up => up.Id == id);

        if (profile == null)
        {
            return NotFound(new { Message = "User profile not found." });
        }

        var identityUser = await _userManager.FindByIdAsync(profile.IdentityUserId);
        var roles = await _userManager.GetRolesAsync(identityUser);

        var userProfileDTO = new UserProfileDTO
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
                AccountType = new AccountTypeDTO
                {
                    Id = a.AccountType.Id,
                    Name = a.AccountType.Name
                }
            }).ToList()
        };

        return Ok(userProfileDTO);
    }
}
