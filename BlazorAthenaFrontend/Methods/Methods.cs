
using BlazorAthenaFrontend.Data;
using BlazorAthenaFrontend.Data.Identity;
using Microsoft.AspNetCore.Identity;
namespace BlazorAthenaFrontend.Methods
{
    public class Methods
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Constructor for the Methods class.
        public Methods(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedIdentityData()
        {
            // Seeding roles and users
            string[] roleNames = { "Admin", "User", "Manager", "Employee" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole { Name = roleName };
                    await _roleManager.CreateAsync(role);
                }
            }

            // Create a default admin user
            if (await _userManager.FindByNameAsync("admin@example.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                };
                await _userManager.CreateAsync(adminUser, "Admin123!");

                // Assign the admin user to the "Admin" role
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create user accounts for Kim, Julia, Joel, Peter, Paul with roles
            var usersWithRoles = new List<(string UserName, string Email, string Password, string Role)>
            {
                ("Kim", "kim@example.com", "Password123!", "User"),
                ("Julia", "julia@example.com", "Password123!", "Manager"),
                ("Joel", "joel@example.com", "Password123!", "Employee"),
                ("Peter", "peter@example.com", "Password123!", "Admin"),
                ("Paul", "paul@example.com", "Password123!", "User")
            };

            foreach (var (userName, email, password, role) in usersWithRoles)
            {
                var user = await _userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    user = new ApplicationUser { UserName = userName, Email = email };
                    await _userManager.CreateAsync(user, password);
                }

                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

        }

    }
}
