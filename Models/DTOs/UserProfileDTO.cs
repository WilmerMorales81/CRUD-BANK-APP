namespace CrudBankApp.Models.DTOs;

public class UserProfileDTO
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public List<AccountDTO> Accounts { get; set; } = new List<AccountDTO>();
}