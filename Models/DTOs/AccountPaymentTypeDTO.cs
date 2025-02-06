using Microsoft.AspNetCore.Identity;

namespace CrudBankApp.Models.DTOs;

public class AccountPaymentTypeDTO
{
    public int AccountId { get; set; }
    public Account Account { get; set; }

    public int PaymentTypeId { get; set; }
    public PaymentTypeDTO PaymentType { get; set; }
}

