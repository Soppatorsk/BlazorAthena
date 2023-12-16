using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorAthenaFrontend.Services
{
    public class CurrentUserService
    {
        public static string GetCurrentUserFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            foreach (var claim in jsonToken?.Claims)
            {
                if (claim.Type == "email")
                {
                    return claim.Value;
                    break;
                }
            }

            return null;
        }
    }
}
