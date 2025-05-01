using MyShopClassLibrary.Models;

namespace MyShop.Data.Entities
{
    public class Reviews
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
