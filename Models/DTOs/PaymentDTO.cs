using System.ComponentModel.DataAnnotations;


namespace CrudBankApp.Models.DTOs
{
    public class PaymentDTO
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
