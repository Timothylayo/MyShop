namespace MyShopClassLibrary.DTOs;

public class UserDTO
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? UserName { get; set; }
    public string? Address { get; set; }
    public string? Role { get; set; }



}
