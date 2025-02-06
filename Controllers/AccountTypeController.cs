using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrudBankApp.Data;
using Microsoft.EntityFrameworkCore;
using CrudBankApp.Models;

namespace CrudBankApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountTypeController : ControllerBase
{
    private readonly CrudBankAppDbContext _dbContext;

    public AccountTypeController(CrudBankAppDbContext context)
    {
        _dbContext = context;
    }

    // GET: api/AccountType
    [HttpGet]
    //[Authorize]
    public IActionResult Get()
    {
        return Ok(_dbContext.AccountTypes.ToList());
    }

    [HttpGet("{id}")]
    //[Authorize]
    public IActionResult GetById(int id)
    {
        AccountType accountType = _dbContext
            .AccountTypes
            .SingleOrDefault(a => a.Id == id);

        if (accountType == null)
        {
            return NotFound();
        }

        return Ok(accountType);
    }



}
