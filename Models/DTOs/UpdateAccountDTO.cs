using System.ComponentModel.DataAnnotations;

namespace CrudBankApp.Models.DTOs
{
  public class UpdateAccountDTO
    {
        public int? AccountTypeId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be greater than or equal to 0")]
        public decimal? Balance { get; set; }
    }
}