namespace MyShop.Data.Entities;

public class OrderDetail
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? OrderId { get; set; }
    public string? ShippingStatus { get; set; }
    public string? ShippingAddress { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal SubTotal { get; set; }
    public decimal? ShippingCharges { get; set; }
    public string? PaymentMode { get; set; }
    public string? CreatedDate { get; set; }
    public string? UpdatedOn { get; set; }
}
