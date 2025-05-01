using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;

namespace MyShop.Client.Services.ProductAdminServices
{
    public interface IAdminService
    {
        //Product
        Task<ServiceResponse> AddProductAsync(Product product);
        Task<List<Product>?> GetAllProductAsync();
        Task<ServiceResponse> DeleteProduct(string Id);
        Task<ServiceResponse> UpdateProduct(Product model);
        Task<Product?> GetProductByIdAsync(string id);
        Task<List<StockModel>> GetProductStock();
        Task<bool> UpdateProductStock(StockModel model);

        //Category
        Task<ServiceResponse> DeleteCategory(int id);
        Task<ServiceResponse> AddCatgeoryAsync(Category model);
        Task<List<Category>> GetCategoriesAsync();

        //Other Services
        Task<List<CartItemDto>?> GetOrderDetails(string order_number);
        Task<List<OrderDTO>?> GetOrders();
    }
}
