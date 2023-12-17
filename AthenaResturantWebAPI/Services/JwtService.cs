using AthenaResturantWebAPI.Data.AppUser;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace AthenaResturantWebAPI.Services
{
    public class JwtService
    {

        private readonly IConfiguration? _iConfiguration;
        public JwtService(IConfiguration Configuration)
        {
            _iConfiguration = Configuration;
        }

        public string GenerateKey() { 
            // Generate a random key
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(key);
            }

            // Convert the byte array to a Base64-encoded string
            string base64Key = Convert.ToBase64String(key);

            Console.WriteLine($"Generated Key: {base64Key}");
            return base64Key;
        }

        public string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecretKey = Encoding.UTF8.GetBytes(_iConfiguration["JwtSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, string.Join(",", roles)),
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_iConfiguration["JwtSettings:TokenExpirationInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecretKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _iConfiguration["JwtSettings:Issuer"],
                Audience = _iConfiguration["JwtSettings:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Validate the generated token
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSecretKey),
                ValidIssuer = _iConfiguration["JwtSettings:Issuer"],
                ValidAudience = _iConfiguration["JwtSettings:Audience"]
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out validatedToken);

            // If you reach this point, the validation was successful
            return tokenString;
        }

        public bool ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecretKey = Encoding.UTF8.GetBytes(_iConfiguration["JwtSettings:SecretKey"]);
            try
            {

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSecretKey),
                    ValidIssuer = _iConfiguration["JwtSettings:Issuer"],
                    ValidAudience = _iConfiguration["JwtSettings:Audience"]
                };
        

                tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private ClaimsPrincipal DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecretKey = Encoding.UTF8.GetBytes(_iConfiguration["JwtSettings:SecretKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSecretKey),
                ValidIssuer = _iConfiguration["JwtSettings:Issuer"],
                ValidAudience = _iConfiguration["JwtSettings:Audience"]
            };

            try
            {
                // Validate and parse the token
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                return principal;
            }
            catch (SecurityTokenException)
            {
                // Token validation failed
                return null;
            }
        }
    }
}
