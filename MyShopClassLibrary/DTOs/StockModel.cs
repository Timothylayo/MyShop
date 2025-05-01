using System.ComponentModel.DataAnnotations;

namespace MyShopClassLibrary.DTOs
{
    public class StockModel
    {
        public int Id { get; set; }
        public int CustomId { get; set; }
        public string? Name { get; set; }
        public int Stock { get; set; }
        [Required(ErrorMessage = "Product Stock is required")]
        public int NewStock { get; set; }
        public string? CategoryName { get; set; }
    }
}
