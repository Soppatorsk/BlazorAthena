using AthenaResturantWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AthenaResturantWebAPI.Models;
using AthenaResturantWebAPI.Data.AppUser;




namespace AthenaResturantWebAPI.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserService userService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
        }



        // GET: api/User
        [HttpGet]
        [Authorize(Roles = "Manager")] 
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var usersWithRoles = await _userService.FetchUsersAsync();
            return Ok(usersWithRoles);
        }

        // DELETE: api/User/{userId}
        [HttpDelete("{userId}")]
        [Authorize(Roles = "Manager")] 
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userService.DeleteUserAsync(userId);
            return Ok(); 
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    }

                    await _userManager.AddToRoleAsync(user, model.Role);

                    return Ok(new { UserId = user.Id });
                }
                else
                {
                    // Handle identity errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Validation failed, return bad request with errors
            return BadRequest(ModelState);
        }



    }
}