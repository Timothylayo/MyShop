using MyShopClassLibrary.Models;
namespace MyShop.Data.Entities;

public class OrderItems
{
    public int Id { get; set; }
    public string? OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal SubTotal { get; set; }
    public string? CreatedDate { get; set; }
    public string? UpdatedOn { get; set; }
    public Product? Product { get; set; }

}
