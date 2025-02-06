using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudBankApp.Models
{
    public class Account
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Number { get; set; }

    private decimal _balance;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance
    {
        get => _balance;
        set
        {
            _balance = value;
            UpdateMinimumPayment();
        }
    }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinPay { get; set; }  // Changed to public set

    [Required]
    public int AccountTypeId { get; set; }
    public AccountType AccountType { get; set; }

    [Required]
    public int UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; }

    public DateTime CreatedAt { get; set; }

    private void UpdateMinimumPayment()
    {
        MinPay = _balance > 0 ? Math.Round(_balance * 0.03m, 2) : 0;
    }
}
}