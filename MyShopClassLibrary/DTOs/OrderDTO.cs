using System.ComponentModel.DataAnnotations;

namespace MyShopClassLibrary.DTOs;

public class OrderDTO
{
    public string UserId { get; set; }
    public string? OrderId { get; set; }
    
    [Required, MaxLength(100)]
    public string? Address { get; set; }
    [Required, Phone]
    public string? PhoneNumber { get; set; }
    public string CreatedDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ShippingStatus { get; set; }
}
