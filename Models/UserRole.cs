using CrudBankApp.Models;

public class UserRole
{
    public int Id { get; set; }
    public string RoleName { get; set; }
    public int UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; }
}
