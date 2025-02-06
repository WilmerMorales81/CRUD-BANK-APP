using Microsoft.AspNetCore.Identity;

namespace CrudBankApp.Models.DTOs;

public class PaymentTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

}
