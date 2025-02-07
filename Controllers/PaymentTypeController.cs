using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrudBankApp.Data;
using Microsoft.EntityFrameworkCore;
using CrudBankApp.Models;

namespace CrudBankApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentTypeController : ControllerBase
{
    private readonly CrudBankAppDbContext _dbContext;

    public PaymentTypeController(CrudBankAppDbContext context)
    {
        _dbContext = context;
    }

    
    [HttpGet]
    //[Authorize]
    public IActionResult Get()
    {
        return Ok(_dbContext.PaymentTypes.ToList());
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetById(int id)
    {
        PaymentType paymentType = _dbContext
            .PaymentTypes
            .SingleOrDefault(a => a.Id == id);

        if (paymentType == null)
        {
            return NotFound();
        }

        return Ok(paymentType);
    }



}
