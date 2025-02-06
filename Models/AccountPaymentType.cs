using Microsoft.AspNetCore.Identity;

namespace CrudBankApp.Models;

public class AccountPaymentType
{
    public int AccountId { get; set; }
    public Account Account { get; set; }

    public int PaymentTypeId { get; set; }
    public PaymentType PaymentType { get; set; }
}

