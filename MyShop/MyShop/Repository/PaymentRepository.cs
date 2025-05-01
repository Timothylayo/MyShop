using MyShopClassLibrary.DTOs;
using Stripe;
using Stripe.Checkout;

namespace MyShop.Repository
{
    public class PaymentRepository(IConfiguration configuration) : IPayment
    {
        private readonly IConfiguration configuration = configuration;

        public Session CreateCheckoutSession(List<CartItemDto> cartItems)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:apikey"];

            var lineItems = new List<SessionLineItemOptions>();

            cartItems.ForEach(c => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = c.Price,
                    Currency = "ngn",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = c.ProductName,
                        Images = [c.ProduuctImageUrl]
                    }
                },
                Quantity = c.Qty,
            }));
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes =
                [
                    "card"
                ],

                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7256/order-success",
                CancelUrl = "https://localhost:7256/cacel",
            };
            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }
    }
}
