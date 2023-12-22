using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorAthenaFrontend.Services
{
    public class JwtDecoderService
    {
        // Method to decode a JWT token and return a ClaimsPrincipal
        public ClaimsPrincipal DecodeToken(string token)
        {
            // Create an instance of JwtSecurityTokenHandler to handle JWT tokens
            var handler = new JwtSecurityTokenHandler();

            // Read and parse the input JWT token
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Create a new ClaimsPrincipal using the claims extracted from the JWT token
            return new ClaimsPrincipal(new ClaimsIdentity(jsonToken?.Claims));
        }
    }
}
