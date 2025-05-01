using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;
using System.Net.Http.Json;

namespace MyShop.Client.Services.AuthenticationService
{
    public class AuthenticationServices(HttpClient httpClient)
    {
        public static bool CheckIfUnauthorized(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return true;
            else
                return false;
        }

        public async Task GetRefreshToken()
        {
            var respone = await httpClient.PostAsJsonAsync<UserSession>("api/User/refresh-token", new UserSession() { ExipiredJWTToken = JWTModel.JWTToken });
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
