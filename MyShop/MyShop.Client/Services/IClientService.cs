using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;

namespace MyShop.Client.Services
{
    public interface IClientService
    {
        //Cart
        Task<ServiceResponse> AddToCartAsync(int Id);
        public Action? CartAction { get; set; }
        public int CartCount { get; set; }
        Task GetCartCount();
        Task<List<CartItemDto>> GetCartItems();
        bool IsCartLoaderVisible { get; set; }
        Task<ServiceResponse> EmptyCart();
        Task<ServiceResponse> UpdateItem(CartItemDto cartItem);
        Task<ServiceResponse> RemoveItem(string removeCartID, int removeProductID);

        //Product
        Action? ProductAction { get; set; }
        Task<List<Product>> GetFeaturedProductAsync();
        Task<List<Product>> GetAllProductAsync();
        Task<List<Product>> GetRecentProductAsync();
        Task<Product> GetProductDescription(string name);
        Task<List<Product>> GetProductByCategory(int Id);
        Task<List<Product>?> GetProductByTextInput(string text);

        //Payment
        Task<string> Checkout();

        //Category
        Action? CategoryAction { get; set; }
        Task<Category[]> GetCategoryAsync();

        //User
        Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO);
        Task<ServiceResponse> RegisterUserAsync(RegisterDTO registerDTO);
        Task<LoginResponse> RefreshToken(UserSession session);

        //Orders
        Task<List<OrderDTO>?> GetOrdersByCustomerId(string customerId);
        Task<List<OrderDTO>?> GetOrders();
        Task<List<CartItemDto>?> GetOrderDetail(string order_number);
        Task<ServiceResponse> UpdateShippingStatus(OrderDTO order);
        Task<UserDTO> GetUserDetails(string userKey);
        Task<ServiceResponse?> UpdateUserDetails(UserDTO user);
        Task<List<string>?> GetShippingStatusForOrder(string order_number);
        Task<OrderDTO?> GetShippingStatus(string order_number);
        Task<List<CartItemDto>?> GetOrderDetailForCustomer(string customerId, string order_number);
        Task<ServiceResponse> ChangePassword(PasswordModel password);
        Task<ServiceResponse> Checkout(List<CartItemDto> cartItems);
        bool IsUserLoggedIn();

    }
}
