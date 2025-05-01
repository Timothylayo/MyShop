using MyShopClassLibrary.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyShopClassLibrary
{
    public static class DecryptJwtToken
    {
        public static CustomClaims GetClaims(string jwtToken)
        {
            try
            {

                if (string.IsNullOrEmpty(jwtToken)) return new CustomClaims();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);

                var id = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)!.Value.ToString();
                var name = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name)!.Value;
                var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email)!.Value;
                var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role)!.Value;

                return new CustomClaims(id, name, email, role);
            }
            catch
            {
                return null!;
            }

        }
    }
}
