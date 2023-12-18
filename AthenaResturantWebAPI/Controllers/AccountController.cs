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
using Microsoft.OpenApi.Validations;


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


    [HttpGet("fetchusers")]
    public async Task<IActionResult> FetchUsers([FromServices] UserManager<ApplicationUser> userManager)
    {
        var users = await userManager.Users.ToListAsync();
        return Ok(users);
    }
    [HttpGet("fetchroles")]

    public async Task<IActionResult> FetchRoles([FromServices] RoleManager<IdentityRole> roleManager)
    {
        // Get a list of all roles.
        var roles = await roleManager.Roles.ToListAsync();

        // Return the dictionary in the response.
        return Ok(roles);
    }



    [HttpGet("fetchusersroles")]
    public async Task<IActionResult> FetchUsersRoles([FromServices] UserManager<ApplicationUser> userManager)
    {
        // Initialize a new list to hold user data and their roles.
        var usersWithRoles = new List<object>();

        // Get a list of all users.
        var users = await userManager.Users.ToListAsync();

        // Iterate over each user.
        foreach (var user in users)
        {
            // Get the role for the current user. Since each user can only have one role,
            // we use the 'FirstOrDefault' method which will return the first role if the user has one,
            // or 'null' if the user has no roles.
            var role = (await userManager.GetRolesAsync(user)).FirstOrDefault();

            // Add an anonymous object containing the user's email and their role to the list.
            usersWithRoles.Add(new { Email = user.Email, Role = role, UserId = user.Id });
        }

        // Return the list of users (by email) and their roles in the response.
        return Ok(usersWithRoles);
    }




    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserById([FromRoute] string userId, [FromServices] UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"User with ID = {userId} not found.");
        }

        return Ok(user);
    }


    [HttpGet("users/{userId}/role")]
    public async Task<IActionResult> GetUserRole([FromRoute] string userId, [FromServices] UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Any())
        {
            var role = new IdentityRole { Name = roles.Single() };
            return Ok(role);
        }
        else
        {
            return NotFound();
        }
    }



    [HttpPost("editusersroles")]
    public async Task<IActionResult> EditUsersRoles([FromBody] RoleOutputModel roleOutputModel, [FromServices] UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByNameAsync(roleOutputModel.UserName);
        if (user == null)
        {
            return NotFound($"User '{roleOutputModel.UserName}' not found.");
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
        var addResult = await userManager.AddToRoleAsync(user, roleOutputModel.RoleName);
        if (!addResult.Succeeded)
        {
            return BadRequest("Failed to add user to role.");
        }

        return Ok($"User '{roleOutputModel.UserName}' has been assigned to role '{roleOutputModel.RoleName}'.");
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

        return Ok(user);
    }
}
