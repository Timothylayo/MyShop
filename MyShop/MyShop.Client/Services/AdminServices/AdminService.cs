using MyShop.Client.Services.AuthenticationService;
using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;
using System.Net.Http.Json;

namespace MyShop.Client.Services.ProductAdminServices
{
    public class AdminService(HttpClient httpClient, AuthenticationServices authenticationServices) : IAdminService
    {
        private readonly AuthenticationServices _authenticationService = authenticationServices;
        private const string AdminBaseUrl = "apiAdmin";

        //Product
        public async Task<ServiceResponse> AddProductAsync(Product product)
        {
            _authenticationService.GetProtectedClient();
            var response = await httpClient.PostAsJsonAsync($"{AdminBaseUrl}/addproduct", product);
            var result = CheckIfUnauthorized(response);
            if (!result.Flag)
            {
                await _authenticationService.GetRefreshToken();
                return await AddProductAsync(product);
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return apiResponse!;
        }

        public async Task<ServiceResponse> DeleteProduct(string Id)
        {
            _authenticationService.GetProtectedClient();
            var response = await httpClient.DeleteAsync($"{AdminBaseUrl}/deleteproduct/{Id}");
            var result = CheckIfUnauthorized(response);
            if (!result.Flag)
            {
                await _authenticationService.GetRefreshToken();
                return await DeleteProduct(Id);
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return apiResponse!;
        }

        public async Task<List<Product>?> GetAllProductAsync()
        {
            _authenticationService.GetProtectedClient();
            var response = await httpClient.GetAsync($"{AdminBaseUrl}/getproducts");
            var result = CheckIfUnauthorized(response);
            if (!result.Flag)
            {
                await _authenticationService.GetRefreshToken();
                return await GetAllProductAsync();
            }
            return await response.Content.ReadFromJsonAsync<List<Product>>();
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            _authenticationService.GetProtectedClient();
            var response = await httpClient.GetAsync($"{AdminBaseUrl}/getproductbyid/{id}");
            var result = CheckIfUnauthorized(response);
            if (!result.Flag)
            {
                await _authenticationService.GetRefreshToken();
                return await GetProductByIdAsync(id);
            }

            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<ServiceResponse> UpdateProduct(Product model)
        {
            _authenticationService.GetProtectedClient();
            var response = await httpClient.PutAsJsonAsync($"{AdminBaseUrl}/updateproduct", model);
            var result = CheckIfUnauthorized(response);
            if (!result.Flag)
            {
                await _authenticationService.GetRefreshToken();
                return await UpdateProduct(model);
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return apiResponse!;
        }

        public async Task<List<StockModel>> GetProductStock()
        {
            _authenticationService.GetProtectedClient();
            var response = await httpClient.GetAsync($"{AdminBaseUrl}/getstock");
            var result = CheckIfUnauthorized(response);
            if (!result.Flag)
            {
                await _authenticationService.GetRefreshToken();
                return await GetProductStock();
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<List<StockModel>>();
            return apiResponse!;
        }

        public async Task<bool> UpdateProductStock(StockModel model)
        {
            _authenticationService.GetProtectedClient();
            var response = await httpClient.PostAsJsonAsync($"{AdminBaseUrl}/updatestock", model);
            var result = CheckIfUnauthorized(response);
            if (!result.Flag)
            {
                await _authenticationService.GetRefreshToken();
                return await UpdateProductStock(model);
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<bool>();
            return apiResponse!;
        }

        //Category
        public async Task<ServiceResponse> AddCatgeoryAsync(Category model)
        {
            GetProtectedClient();
            var response = await httpClient.PostAsJsonAsync($"{AdminBaseUrl}/addcategory", model);
            var check = CheckIfUnauthorized(response);
            if (!check.Flag)
            {
                await GetRefreshToken();
                await AddCatgeoryAsync(model);
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return apiResponse!;
        }

        public async Task<ServiceResponse> DeleteCategory(int id)
        {
            GetProtectedClient();
            var response = await httpClient.DeleteFromJsonAsync<ServiceResponse>($"{AdminBaseUrl}/deletecategory/{id}");
            if (response == null)
            {
                await GetRefreshToken();
                await DeleteCategory(id);
            }
            return response!;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var category = await httpClient.GetFromJsonAsync<List<Category>>($"{AdminBaseUrl}/getcategories");
            return category!;
        }
        //Other Services
        public async Task<List<CartItemDto>?> GetOrderDetails(string order_number)
        {
            return await httpClient.GetFromJsonAsync<List<CartItemDto>>($"{AdminBaseUrl}/getorderdetails/{order_number}");
        }

        public async Task<List<OrderDTO>?> GetOrders()
        {
            return await httpClient.GetFromJsonAsync<List<OrderDTO>>($"{AdminBaseUrl}/getorders");
        }

        //Private methods
        private static ServiceResponse CheckIfUnauthorized(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return new ServiceResponse(false, "Error occurred. Try Again Later...");
            else
                return new ServiceResponse(true, null!);
        }

        public async Task GetRefreshToken()
        {
            var respone = await httpClient.PostAsJsonAsync($"{AdminBaseUrl}/refresh-token", new UserSession() { ExipiredJWTToken = JWTModel.JWTToken });
            var apiResponse = await respone.Content.ReadFromJsonAsync<LoginResponse>();
            JWTModel.JWTToken = apiResponse!.Token;
        }

        public void GetProtectedClient()
        {
            if (JWTModel.JWTToken == "") return;
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JWTModel.JWTToken);
        }
    }
}
