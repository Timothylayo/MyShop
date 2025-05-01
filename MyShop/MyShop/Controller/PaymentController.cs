using Microsoft.AspNetCore.Mvc;
using MyShop.Repository;
using MyShopClassLibrary.DTOs;

namespace MyShop.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IPayment payment) : ControllerBase
    {
        private readonly IPayment payment = payment;

        [HttpPost("checkout")]
        public ActionResult CheckoutStripe(List<CartItemDto> cartItems)
        {
            var data = payment.CreateCheckoutSession(cartItems);
            return Ok(data);
        }
    }
}
