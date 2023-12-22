using AthenaResturantWebAPI.Data.AppUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using AthenaResturantWebAPI.Models;

namespace AthenaResturantWebAPI.Services
{
    public class UserService
    {

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager; 

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<GetUserRoleModels>> FetchUsersAsync()
        {
            var usersWithRoles = new List<GetUserRoleModels>();

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = new GetUserRoleModels
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                };

                usersWithRoles.Add(userRole);
            }

            return usersWithRoles;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<string> CreateUserAsync(string email, string password, string roleName)
        {
            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Check if the role exists, and create it if not
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }

                // Add user to the specified role
                await _userManager.AddToRoleAsync(user, roleName);

                return user.Id;
            }

            // Handle errors in result.Errors
            return null;
        }
    }

}
