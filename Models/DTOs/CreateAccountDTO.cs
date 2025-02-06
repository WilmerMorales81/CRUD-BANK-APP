using System.ComponentModel.DataAnnotations;


namespace CrudBankApp.Models.DTOs
{
    public class CreateAccountDTO
    {
        [Required]
        public int AccountTypeId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Initial balance must be greater than or equal to 0")]
        public decimal InitialBalance { get; set; }
    }
}