using System.ComponentModel.DataAnnotations;

namespace CrudBankApp.Models.DTOs
{
    public class AccountDTO
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Number { get; set; }
    
    [Required]
    public int AccountTypeId { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Balance must be greater than or equal to 0")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal Balance { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Minimum payment must be greater than or equal to 0")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal MinPay { get; set; }

    public int UserProfileId { get; set; }
    public UserProfileDTO UserProfile { get; set; }  // Add this line
    
    public string AccountTypeName { get; set; }
    public AccountTypeDTO AccountType { get; set; }
    public DateTime CreatedAt { get; set; }

    // Formatted properties for display
    public string FormattedBalance => Balance.ToString("C2");
    public string FormattedMinPay => MinPay.ToString("C2");
    public string MinPaymentPercentage => "3%";
    public bool HasBalance => Balance > 0;
}
}