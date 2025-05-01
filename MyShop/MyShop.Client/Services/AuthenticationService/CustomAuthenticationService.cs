using Microsoft.AspNetCore.Components.Authorization;
using MyShopClassLibrary;
using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using System.Security.Claims;

namespace MyShop.Client.Services.AuthenticationService
{
    public class CustomAuthenticationStateProvider() : AuthenticationStateProvider
    {

        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            if (string.IsNullOrEmpty(JWTModel.JWTToken))
                return await Task.FromResult(new AuthenticationState(anonymous));

            var getUserClaims = DecryptJwtToken.GetClaims(JWTModel.JWTToken);


            var claims = SetClaimsPrincipal(getUserClaims);
            if (claims is null)
                return await Task.FromResult(new AuthenticationState(anonymous));
            else
                return await Task.FromResult(new AuthenticationState(claims));

        }

        //set claims
        public static ClaimsPrincipal SetClaimsPrincipal(CustomClaims claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();
            return new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new(ClaimTypes.Name, claims.Name!),
                    new(ClaimTypes.Email, claims.Email!),
                    new(ClaimTypes.Role, claims.Role!),

                ], "JwtAuth"));
        }

        // get claims


        public void UpdateAuthenticationState(string jwtToken)
        {
            var claims = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                JWTModel.JWTToken = jwtToken;
                var getUserClaims = DecryptJwtToken.GetClaims(jwtToken);
                var setClaims = SetClaimsPrincipal(getUserClaims);
                if (setClaims is null) return;
            }
            else
            {
                JWTModel.JWTToken = null!;
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claims)));
        }
    }
}
