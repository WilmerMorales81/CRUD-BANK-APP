using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CrudBankApp.Models
{
    public class AccountType
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        // One-to-many relationship: One account type can have multiple accounts
        [JsonIgnore]
        public ICollection<Account> Accounts { get; set; }

        public AccountType()
        {
            Accounts = new List<Account>();
        }
    }
}