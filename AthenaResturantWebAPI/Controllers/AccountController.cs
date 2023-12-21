using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AthenaResturantWebAPI.Services;
using System.Threading.Tasks;
using AthenaResturantWebAPI.Data.AppUser;
using AthenaResturantWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;


public class AccountController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, JwtService jwtService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtService = jwtService;
    }
    public ClaimsPrincipal DecodeToken(string token)
    {
        // Create an instance of JwtSecurityTokenHandler to handle JWT tokens
        var handler = new JwtSecurityTokenHandler();

        // Read and parse the input JWT token
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        // Create a new ClaimsPrincipal using the claims extracted from the JWT token
        return new ClaimsPrincipal(new ClaimsIdentity(jsonToken?.Claims));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return Unauthorized();
                }

                // Generate the JWT token
                var token = _jwtService.GenerateJwtToken(user, roles);
                
                // Set the authentication cookie or token in the response
                Response.Cookies.Append("Authorization", $"Bearer {token}", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    // You may need to adjust other options based on your security requirements
                });
                var decode = DecodeToken(token);
                // Return the JWT token directly
                return Ok(new { Token = token });
            }
            else
            {
                // Authentication failed
                return BadRequest("Invalid login attempt");
            }
        }

        // Model is not valid
        return BadRequest("Invalid model");
    }



    [HttpGet("fetchusers")]
    public async Task<IActionResult> FetchUsers([FromServices] UserManager<ApplicationUser> userManager)
    {
        var users = await userManager.Users.ToListAsync();
        return Ok(users);
    }





    [HttpPost("editusersroles")]
    public async Task<IActionResult> EditUsersRoles([FromBody] RoleOutputModel roleOutput, [FromServices] UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByNameAsync(roleOutput.UserName);
        if (user == null)
        {
            return NotFound($"User '{roleOutput.UserName}' not found.");
        }

        // Get the current roles attached to the user
        var currentRoles = await userManager.GetRolesAsync(user);

        // Remove all roles associated with the user
        var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return BadRequest("Failed to remove user roles.");
        }

        // Assign the user to the new role
        var addResult = await userManager.AddToRoleAsync(user, roleOutput.RoleName);
        if (!addResult.Succeeded)
        {
            return BadRequest("Failed to add user to role.");
        }

        return Ok($"User '{roleOutput.UserName}' has been assigned to role '{roleOutput.RoleName}'.");
    }




    [HttpPost("editusers")]
    public async Task<IActionResult> EditUsers([FromBody] ApplicationUser updatedUser, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager)
    {
        if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Id))
        {
            return BadRequest("Invalid user data.");
        }

        // Fetch the user from the database
        var user = await userManager.FindByIdAsync(updatedUser.Id);

        if (user == null)
        {
            return NotFound($"User with ID = {updatedUser.Id} not found.");
        }

        // Update the user properties
        user.Email = updatedUser.Email ?? user.Email;
        user.UserName = updatedUser.UserName ?? user.UserName;
        // Add other properties that you want to update

        // Save the changes
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest("User update failed.");
        }

        // Assume userManager is an instance of UserManager<TUser>
        var oldRole = await userManager.GetRolesAsync(user);
        var result1 = await userManager.AddToRoleAsync(user, "Manager"); //roleOutput.RoleName

        // Removing a user from a role
        var result2 = await userManager.RemoveFromRoleAsync(user, oldRole.ToString());


        return Ok(user);
    }













    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        // Check if the user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            // Get the user's ID from the claims
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                // User is not authenticated
                return Unauthorized();
            }

            // Retrieve the user from UserManager using their ID
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // User not found
                return NotFound();
            }

            // Now 'user' contains the IdentityUser object for the current user
            return Ok(new
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IsAuthenticated = true
            });
        }
        else
        {
            // User is not authenticated
            return Unauthorized();
        }
    }

   
}
