using CrudBankApp.Models.DTOs;

public class UserRoleDTO
{
    public int Id { get; set; }
    public string RoleName { get; set; }
    public int UserProfileId { get; set; }
    public UserProfileDTO UserProfile { get; set; }
}
