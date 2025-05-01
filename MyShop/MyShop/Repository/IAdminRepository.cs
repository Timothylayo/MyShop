using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;

namespace MyShop.Repository
{
    public interface IAdminRepository
    {
        Task<ServiceResponse> AddCatgeoryAsync(Category model);
        Task<ServiceResponse> AddProductAsync(Product product);
        Task<ServiceResponse> DeleteCategory(int id);
        Task<ServiceResponse> DeleteProduct(string Id);
        Task<List<Product>> GetAllProductAsync();
        Task<List<Category>> GetCategoryAsync();
        Task<Category> GetCategoryAsyncById(int Id);
        List<CartItemDto> GetOrderDetails(string order_number);
        Task<Product?> GetProductByIdAsync(string prodId);
        List<StockModel> GetProductStock();
        Task<ServiceResponse> UpdateProduct(Product model);
        bool UpdateStock(StockModel model);
        List<OrderDTO> GetOrders();
        LoginResponse RefreshToken(UserSession session);
    }
}