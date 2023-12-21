using AthenaResturantWebAPI.Data.AppUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AthenaResturantWebAPI.Models;

namespace AthenaResturantWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public IdentityController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("token")]
        public async Task<IActionResult> PostGenerateJwtToken([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateToken(user, roles);

            //// Decode the token to verify its content
            var principal = DecodeToken(token);
            // IsAuthenticated = True
            if (principal != null)
            {
                // Token is valid, proceed with your logic
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                // Do something with userId
                return Ok(new { Token = token }); ;
            }
            else
            {
                // Token is invalid
                return Unauthorized();
            }
            return Ok(new { Token = token });
        }

        private string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var expires = DateTime.Now.AddHours(1);
            var role = roles[0];
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecretKey = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.Authentication, "true")
                }),
                Expires = expires, 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecretKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
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
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"]
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out validatedToken);

            // If you reach this point, the validation was successful
            return tokenString;
        }
        private string GenerateKey()
        {
            // Generate a random key with the required length (32 bytes)
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

        private ClaimsPrincipal DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecretKey = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSecretKey),
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"]
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
