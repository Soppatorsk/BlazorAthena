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

        private string GenerateKey() { 
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

        public string GenerateJwtToken(string userId, string userEmail, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecretKey = Encoding.UTF8.GetBytes(GenerateKey());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role, string.Join(",", roles)),
                }),

                // Set the NotBefore timestamp to the current time
                NotBefore = DateTime.UtcNow,

                // Set the Expires timestamp to a future time
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_iConfiguration["Jwt:TokenExpirationInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_iConfiguration["Jwt:SecretKey"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
