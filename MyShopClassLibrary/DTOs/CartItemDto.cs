namespace MyShopClassLibrary.DTOs
{
    public class CartItemDto
    {
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public int AvailableStock { get; set; }
        public int Qty { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string ProduuctImageUrl { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMode { get; set; }
        public decimal ShippingCharges { get; set; }


    }
}
