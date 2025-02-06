using System.ComponentModel.DataAnnotations;

namespace CrudBankApp.Models.DTOs
{
    public class UpdateUserProfileDTO
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        [Required]
        [StringLength(15)]
        [Phone]
        public string Phone { get; set; }
    }
}