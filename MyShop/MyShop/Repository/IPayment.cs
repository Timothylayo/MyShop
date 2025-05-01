using MyShopClassLibrary.DTOs;
using Stripe.Checkout;

namespace MyShop.Repository
{
    public interface IPayment
    {
        Session CreateCheckoutSession(List<CartItemDto> cartItems);
    }
}
