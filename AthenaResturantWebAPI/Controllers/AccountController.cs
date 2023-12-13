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


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
             
                // Authentication succeeded
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                // Generate the JWT token
                var token = _jwtService.GenerateJwtToken(user.Id, user.Email, roles.ToList());

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

    /*
    [HttpPost("fetchusers")]
    public async Task<IActionResult> FetchUsers()
    {

    }*/

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
        });
    }

    [HttpGet("current-user1")]
    public async Task<IActionResult> GetCurrentUserInfo1()
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

        // Retrieve the user's roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate the JWT token
        var token = _jwtService.GenerateJwtToken(user.Id, user.Email, roles.ToList());

        // Now 'user' contains the IdentityUser object for the current user
        return Ok(new
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Token = token,
            // Add any other properties you want to expose
        });
    }
}
