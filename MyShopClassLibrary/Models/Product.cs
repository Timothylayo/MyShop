using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyShopClassLibrary.Models;

public class Product
{
    public int Id { get; set; }
    public string? ProdNum { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    [Required, DisplayName("Product Image")]
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsActive { get; set; }
    public bool Featured { get; set; } = true;
    public int ViewCount { get; set; }
    public string? Brand { get; set; }
    [Required, Range(0.1, 9999999.99)]
    public decimal OriginalPrice { get; set; }
    [Required, Range(0.1, 9999999.99)]
    public decimal Price { get; set; }

    public virtual Category? Category { get; set; }
}
