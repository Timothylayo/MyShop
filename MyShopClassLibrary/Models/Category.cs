using System.ComponentModel.DataAnnotations;

namespace MyShopClassLibrary.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    public Category Clone() => (MemberwiseClone() as Category)!;
}
