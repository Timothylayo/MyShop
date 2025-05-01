namespace MyShopClassLibrary.DTOs
{
    public class OrderItemsDTO
    {
        public string? OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedOn { get; set; }

    }
}
