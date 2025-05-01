using System.ComponentModel.DataAnnotations;


namespace MyShop.Data.Entities;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUsers
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? PasswordHash { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserName { get; set; }
    public string? Address { get; set; }
    public string? Role { get; set; }
    public ICollection<OrderDetail> Orders { get; set; }
    public ICollection<Reviews> Reviews { get; set; }
}
