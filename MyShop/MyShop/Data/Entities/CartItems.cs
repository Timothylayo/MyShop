using MyShopClassLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace MyShop.Data.Entities;

public class CartItems
{
    [Key]
    public int ItemId { get; set; }
    public string? CartId { get; set; }
    public int Quantity { get; set; }
    public DateTime DateCreated { get; set; }
    public int ProductId {  get; set; }
    public string? UserId { get; set; }
    public Product? Product { get; set; }
}
