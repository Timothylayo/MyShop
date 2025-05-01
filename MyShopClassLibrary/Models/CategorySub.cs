using System.ComponentModel.DataAnnotations;

namespace MyShopClassLibrary.Models;

public class CategorySub
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
