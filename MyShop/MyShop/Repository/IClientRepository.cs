using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;

namespace MyShop.Repository
{
    public interface IClientRepository
    {
        //Product
        Task<List<Product>> GetAllProductAsync();
        Task<List<Product>> GetFeaturedProductsAsync();
        Task<List<Product>> GetNewArrival();
        Task<List<Product>> GetProductByCategory(int categoryId);
        Task<Product?> GetProductByIdAsync(string Id);
        Task<List<Product>> GetProductByTextInput(string text);
        Task<Product> GetProductDescription(string name);
        Task<List<Product>> GetProductsByQuery(string? query = null);

        //Cart
        Task<ServiceResponse> AddToCart(int id);
        Task<ServiceResponse> EmptyCart();
        List<CartItemDto> GetCartItems();
        Task<ServiceResponse> UpdateItem(CartItemDto cartItem);
        Task<ServiceResponse> RemoveItem(string removeCartID, int removeProductID);

        //Category
        Task<List<Category>> GetCategoryAsync();
        Task<Category> GetCategoryAsyncById(int Id);

        //Order
        List<CartItemDto> GetOrderDetailForCustomer(string customerId, string order_number);
        List<CartItemDto> GetOrderDetails(string order_number);
        List<OrderDTO> GetOrders();
        List<OrderDTO> GetOrdersByCustomerId(string customerId);
        OrderDTO GetShippingStatus(string order_number);
        List<string> GetShippingStatusForOrder(string order_number);
        Task<ServiceResponse> UpdateShippingStatus(OrderDTO orderDTO);
        Task<ServiceResponse> Checkout(List<CartItemDto> cartItems);

        //User
        ServiceResponse ChangePassword(PasswordModel password);
        Task<UserDTO> GetUserDetails(string userKey);
        Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO);
        LoginResponse RefreshToken(UserSession session);
        Task<ServiceResponse> RegisterUserAsync(RegisterDTO registerDTO);
        Task<ServiceResponse> UpdateUserDetails(UserDTO user);

        //Email
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}