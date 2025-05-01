using Microsoft.AspNetCore.Components;
using MyShop.Client.Services.AuthenticationService;
using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;
using System.Net.Http.Json;

namespace MyShop.Client.Services
{
    public class ClientService(HttpClient httpClient, NavigationManager navigationManager, AuthenticationServices authenticationService) : IClientService
    {

        private const string ClientBaseURL = "api/Client";
        private readonly HttpClient httpClient = httpClient;
        private readonly NavigationManager navigationManager = navigationManager;
        private readonly AuthenticationServices _authenticationService = authenticationService;

        public Action? CategoryAction { get; set; }
        public Action? ProductAction { get; set; }
        public List<Product>? ProductsByCatgeory { get; set; }
        public Action? CartAction { get; set; }
        public int CartCount { get; set; }
        public bool IsCartLoaderVisible { get; set; }

        //Products
        public async Task<Product> GetProductByIdAsync(string Id)
        {
            var response = await httpClient.GetFromJsonAsync<Product>($"{ClientBaseURL}/get-by/{Id}");
            return response!;
        }

        public async Task<List<Product>> GetFeaturedProductAsync()
        {
            var product = await httpClient.GetFromJsonAsync<List<Product>>($"{ClientBaseURL}/featured-product");
            return product!;
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            var AllProducts = await httpClient.GetFromJsonAsync<List<Product>>($"{ClientBaseURL}/all-product");

            return AllProducts!;
        }

        public Task<List<Product>> GetRecentProductAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetProductDescription(string name)
        {
            var product = await httpClient.GetFromJsonAsync<Product>($"{ClientBaseURL}/{name}");
            return product!;
        }

        public async Task<List<Product>> GetProductByCategory(int Id)
        {
            var product = await httpClient.GetFromJsonAsync<List<Product>>($"{ClientBaseURL}/byCategory/{Id}");
            return product!;
        }

        public async Task<List<Product>?> GetProductByTextInput(string text)
        {
            return await httpClient.GetFromJsonAsync<List<Product>>($"{ClientBaseURL}/search/{text}");
        }

        //Category
        public async Task<Category[]> GetCategoryAsync()
        {
            var category = await httpClient.GetFromJsonAsync<Category[]>("{getcategories}");
            return category!;

        }

        //Cart

        public async Task<ServiceResponse> AddToCartAsync(int Id)
        {
            if (JWTModel.JWTToken == null)
            {
                navigationManager.NavigateTo($"account/login");
            }
            else
            {
                var cartitems = await httpClient.PostAsJsonAsync($"{ClientBaseURL}/addcartitems/{Id}", Id);

                var apiResponse = await cartitems.Content.ReadFromJsonAsync<ServiceResponse>();
                //_toastService.ShowSuccess(apiResponse!.Message);
            }
            await GetCartCount();
            return new ServiceResponse(true, "Product Added to Cart");
        }

        public async Task<ServiceResponse> EmptyCart()
        {
            var result = await httpClient.DeleteFromJsonAsync<ServiceResponse>($"{ClientBaseURL}/emptycart");
            return result!;
        }

        public async Task<List<CartItemDto>> GetCartItems()
        {
            var result = await httpClient.GetFromJsonAsync<List<CartItemDto>>($"{ClientBaseURL}/getcartitems");
            return result!;
        }

        public async Task<ServiceResponse> UpdateItem(CartItemDto cartItem)
        {
            var result = await httpClient.PutAsJsonAsync($"{ClientBaseURL}/updatecart", cartItem);
            var apiResponse = await result.Content.ReadFromJsonAsync<ServiceResponse>();
            return apiResponse!;
        }

        public async Task<ServiceResponse> RemoveItem(string removeCartID, int removeProductID)
        {
            var result = await httpClient.DeleteFromJsonAsync<ServiceResponse>($"{ClientBaseURL}/removeitem/{removeCartID}/{removeProductID}");
            return result!;
        }

        public async Task GetCartCount()
        {
            var cart = await GetCartItems();
            if (cart == null)
                CartCount = 0;
            else
                CartCount = cart.Count;

            CartAction?.Invoke();
        }

        //Payment
        public async Task<string> Checkout()
        {
            var result = await httpClient.PostAsJsonAsync($"{ClientBaseURL}/checkout", await GetCartItems());
            var url = await result.Content.ReadAsStringAsync();
            return url!;
        }

        //User
        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO)
        {
            var response = await httpClient.PostAsJsonAsync($"{ClientBaseURL}/login", loginDTO);
            if (!response.IsSuccessStatusCode)
            {
                return new LoginResponse(false, "Error Occured", null!);
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return apiResponse!;
        }

        public async Task<LoginResponse> RefreshToken(UserSession session)
        {
            var respone = await httpClient.PostAsJsonAsync($"{ClientBaseURL}/refresh-token", session);
            var apiResponse = await respone.Content.ReadFromJsonAsync<LoginResponse>();
            return apiResponse!;
        }

        public async Task<ServiceResponse> RegisterUserAsync(RegisterDTO registerDTO)
        {

            var response = await httpClient.PostAsJsonAsync($"{ClientBaseURL}/register", registerDTO);
            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse(false, "Error Occured");
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return apiResponse!;

        }

        public async Task<ServiceResponse> ChangePassword(PasswordModel password)
        {
            var response = await httpClient.PostAsJsonAsync($"{ClientBaseURL}/ChangePassword", password);
            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse(false, "Error Occured");
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return apiResponse!;
        }

        //Order
        public async Task<List<OrderDTO>?> GetOrdersByCustomerId(string customerId)
        {
            return await httpClient.GetFromJsonAsync<List<OrderDTO>>($"{ClientBaseURL}/GetOrdersByCustomerId/{customerId}");
        }

        public async Task<List<CartItemDto>?> GetOrderDetailForCustomer(string customerId, string order_number)
        {
            return await httpClient.GetFromJsonAsync<List<CartItemDto>>($"{ClientBaseURL}/GetOrderDetailForCustomer/{customerId}/{order_number}");
        }

        public async Task<List<CartItemDto>?> GetOrderDetail(string order_number)
        {
            return await httpClient.GetFromJsonAsync<List<CartItemDto>>($"{ClientBaseURL}/GetOrderDetails/{order_number}");
        }

        public async Task<ServiceResponse> UpdateShippingStatus(OrderDTO order)
        {
            _authenticationService.GetProtectedClient();
            var response = await httpClient.PutAsJsonAsync($"{ClientBaseURL}/update-status", order);
            var result = CheckIfUnauthorized(response);
            if (result)
            {
                await _authenticationService.GetRefreshToken();
                return await UpdateShippingStatus(order);
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return apiResponse!;
        }

        public async Task<List<string>?> GetShippingStatusForOrder(string order_number)
        {
            return await httpClient.GetFromJsonAsync<List<string>>($"{ClientBaseURL}/GetShippingStatusForOrder/{order_number}");
        }

        public Task<OrderDTO?> GetShippingStatus(string order_number)
        {
            return httpClient.GetFromJsonAsync<OrderDTO>($"{ClientBaseURL}/GetShippingStatus/{order_number}");
        }

        public async Task<UserDTO> GetUserDetails(string userKey)
        {
            var response = await httpClient.GetAsync($"{ClientBaseURL}/user-info/{userKey}");
            var apiResponse = await response.Content.ReadFromJsonAsync<UserDTO>();
            return apiResponse!;
        }

        public async Task<ServiceResponse?> UpdateUserDetails(UserDTO user)
        {
            _authenticationService.GetProtectedClient();
            var response = await httpClient.PutAsJsonAsync($"{ClientBaseURL}/update/user-info", user);
            bool check = CheckIfUnauthorized(response);
            if (check)
            {
                await _authenticationService.GetRefreshToken();
                await GetUserDetails(user.Id!);
            }
            return await response.Content.ReadFromJsonAsync<ServiceResponse>();
        }

        public async Task<ServiceResponse> Checkout(List<CartItemDto> cartItems)
        {
            var response = await httpClient.PostAsJsonAsync($"{ClientBaseURL}/checkout", cartItems);
            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse(false, "Error Occured");
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return apiResponse!;
        }

        public async Task<List<OrderDTO>?> GetOrders()
        {
            return await httpClient.GetFromJsonAsync<List<OrderDTO>>($"{ClientBaseURL}/GetOrders");

        }



        //Private Methods
        public static bool CheckIfUnauthorized(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return true;
            else
                return false;
        }

        public bool IsUserLoggedIn()
        {
            bool flag;
            if (string.IsNullOrEmpty(JWTModel.JWTToken))
            {
                flag = false;
            }
            else
            {
                flag = true;
            }
            return flag;
        }
        public void GetProtectedClient()
        {
            if (JWTModel.JWTToken == "") return;
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JWTModel.JWTToken);
        }

    }
}
