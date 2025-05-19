using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Repository;
using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;

namespace MyShop.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController(IClientRepository clientRepository) : ControllerBase
    {
        private readonly IClientRepository _clientRepository = clientRepository;


        //User
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LogUserIn(LoginDTO login)
        {
            var result = await _clientRepository.LoginUserAsync(login);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public ActionResult<LoginResponse> RefreshToken(UserSession model)
        {
            var result = _clientRepository.RefreshToken(model);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse>> Register(RegisterDTO register)
        {
            var result = await _clientRepository.RegisterUserAsync(register);
            return Ok(result);
        }

        [HttpGet("user-info/{userKey}")]
        public async Task<ActionResult<UserDTO>> GetUserInfo(string userKey)
        {
            var getUser = await _clientRepository.GetUserDetails(userKey);
            return Ok(getUser);
        }

        [HttpPut("update/user-info")]
        [Authorize]
        public async Task<ActionResult> UpdateUserInfo(UserDTO userDTO)
        {
            var getUser = await _clientRepository.UpdateUserDetails(userDTO);
            return Ok(getUser);
        }

        [HttpPost("ChangePassword")]
        public ActionResult ChangePassword(PasswordModel model)
        {
            var data = _clientRepository.ChangePassword(model);
            return Ok(data);
        }

        //Order
        [HttpPut("update-status")]
        public async Task<ActionResult> UpdateShippingStatus(OrderDTO orderDTO)
        {
            var updateStatus = await _clientRepository.UpdateShippingStatus(orderDTO);
            return Ok(updateStatus);
        }

        [HttpGet("GetOrdersByCustomerId/{customerId}")]
        public ActionResult GetOrdersByCustomerId(string customerId)
        {
            var data = _clientRepository.GetOrdersByCustomerId(customerId);
            return Ok(data);
        }

        [HttpGet("GetOrders")]
        public ActionResult GetOrdersByCustomerId()
        {
            var data = _clientRepository.GetOrders();
            return Ok(data);
        }

        [HttpGet("GetOrderDetailForCustomer/{customerId}/{order_number}")]
        public ActionResult GetOrderDetailForCustomer(string customerId, string order_number)
        {
            var data = _clientRepository.GetOrderDetailForCustomer(customerId, order_number);
            return Ok(data);
        }

        [HttpGet("GetOrderDetails/{order_number}")]
        public ActionResult GetOrderDetails(string order_number)
        {
            var data = _clientRepository.GetOrderDetails(order_number);
            return Ok(data);
        }

        [HttpGet("GetShippingStatusForOrder/{order_number}")]
        public ActionResult GetShippingStatusForOrder(string order_number)
        {
            var data = _clientRepository.GetShippingStatusForOrder(order_number);
            return Ok(data);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<ServiceResponse>> CheckOut(List<CartItemDto> cartItems)
        {
            var data = await _clientRepository.Checkout(cartItems);
            return Ok(data);
        }

        //Product
        [HttpGet("all-product")]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var result = await _clientRepository.GetAllProductAsync();
            return Ok(result);
        }

        [HttpGet("search/{text}")]
        public async Task<ActionResult<List<Product>>> GetAllProductsByTextInput(string text)
        {
            return Ok(await _clientRepository.GetProductByTextInput(text));
        }

        [HttpGet("featured-product")]
        public async Task<ActionResult<List<Product>>> GetFeaturesProducts()
        {
            var result = await _clientRepository.GetFeaturedProductsAsync();
            return Ok(result);
        }

        [HttpGet("name")]
        public async Task<ActionResult<Product>> GetProductsByDescription(string name)
        {
            var result = await _clientRepository.GetProductDescription(name);
            return Ok(result);
        }

        [HttpGet("get-by/{Id}")]
        public async Task<ActionResult<Product>> GetProductById(string Id)
        {
            var getProductById = await _clientRepository.GetProductByIdAsync(Id);
            return Ok(getProductById);
        }

        [HttpGet("byCategory/{Id}")]
        public async Task<ActionResult<List<Product>>> GetProductByCategory(int Id)
        {
            var result = await _clientRepository.GetProductByCategory(Id);
            return Ok(result);
        }

        //Category
        [HttpGet("getcategories")]
        public async Task<ActionResult<Category[]>> GetCategoriesAsync()
        {
            var categories = await _clientRepository.GetCategoryAsync();
            return Ok(categories);
        }

        [HttpGet("getcatgeorybyid/{Id}")]
        public async Task<ActionResult> GetCategoryByIdAsync(int Id)
        {
            return Ok(await _clientRepository.GetCategoryAsyncById(Id));
        }

        //Cart
        [HttpGet("getcartitems")]
        public ActionResult<List<CartItemDto>> GetCartItems()
        {
            var category = _clientRepository.GetCartItems();
            return Ok(category);
        }

        [HttpPost("addcartitems/{Id}")]
        public async Task<ActionResult<ServiceResponse>> AddToCart(int Id)
        {
            var cart = await _clientRepository.AddToCart(Id);
            return Ok(cart);
        }

        [HttpDelete("emptycart")]
        public async Task<ActionResult<ServiceResponse>> EmptyCart()
        {
            return Ok(await _clientRepository.EmptyCart());
        }

        [HttpPut("updatecart")]
        public async Task<ActionResult<ServiceResponse>> UpdateCart(CartItemDto cartItem)
        {
            return Ok(await _clientRepository.UpdateItem(cartItem));
        }

        [HttpDelete("removeItem/{CartId}/{productId}")]
        public async Task<ActionResult<ServiceResponse>> RemoveItemAsync(string CartId, int productId)
        {
            return Ok(await _clientRepository.RemoveItem(CartId, productId));
        }
    }
}
