using Microsoft.AspNetCore.Identity;

namespace CrudBankApp.Models;

public class CreateAccountRequest
{

    public decimal InitialBalance { get; set; }
    public int AccountTypeId { get; set; }
}
