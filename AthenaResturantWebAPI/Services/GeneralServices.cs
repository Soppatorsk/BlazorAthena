using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BlazorAthena.Models;
using AthenaResturantWebAPI.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using AthenaResturantWebAPI.Data.AppUser;
using System.Security.Claims;
using Humanizer;
using AthenaResturantWebAPI.Migrations;
using AthenaResturantWebAPI.Models;


namespace AthenaResturantWebAPI.Services
{
    public class GeneralServices : ControllerBase
    {

      
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public GeneralServices( AppDbContext appDbContext,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)

        {

            _context = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        public async Task<IActionResult> CheckUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"User Roles: {string.Join(", ", userRoles)}");

                var isInRole = await _userManager.IsInRoleAsync(user, roleName);

                if (isInRole)
                {
                    return Ok($"{user.UserName} is in the {roleName} role.");
                }
                else
                {
                    return Ok($"{user.UserName} is not in the {roleName} role.");
                }
            }

            return BadRequest("User not found");
        }

        public async Task SeedData(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            try
            {
                //var userID = "627764fe-39e4-4d51-88fc-0330207254d0";
                //var roleName = "Employee";

                //await CheckUserRole(userID,roleName);
             

                if (!context.Products.Any())
                {
                    // Seed Roles 
                    await SeedRoles(roleManager);
                    _context.SaveChanges();
                    // Seed product subtables (food, drink, merch)
                    await AllergiesAndDrinks(context);
                    await SeedMerch(context);
                    // Seed Products
                    await AssignSubCategoryId(context);
                    _context.SaveChanges();
                    // Seed Users
                    await SeedUsersAsync(userManager);
                    _context.SaveChanges();
                    await AssignUserRoleAsync(userManager, roleManager, "kim@example.com", "Employee");
                    await AssignUserRoleAsync(userManager, roleManager, "julia@example.com", "Manager");
                    await AssignUserRoleAsync(userManager, roleManager, "joel@example.com", "Admin");
                    await AssignUserRoleAsync(userManager, roleManager, "simon@example.com", "Admin");
                    await AssignUserRoleAsync(userManager, roleManager, "peter@example.com", "Manager");
                    await AssignUserRoleAsync(userManager, roleManager, "paul@example.com", "User");
                    // Seed Claims / Roles
                    await SeedRoleClaimsAsync(roleManager);
                    _context.SaveChanges();
                    // Seeds Order & OrderLine
                    await CreateOrder(context);

                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected exception (log, throw, etc.)
                Console.WriteLine($"An error occurred during seeding: {ex.Message}");
            }
        }

        private static async Task SeedRoleClaimAsync(RoleManager<IdentityRole> roleManager, string roleName, string claimValue)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if (role != null)
            {
                var existingClaims = await roleManager.GetClaimsAsync(role);

                // Check if the claim is not already assigned to the role
                if (!existingClaims.Any(c => c.Type == "custom_claim_type" && c.Value == claimValue))
                {
                    var newClaim = new Claim("custom_claim_type", claimValue, ClaimValueTypes.String);

                    // Assign the claim to the role
                    var result = await roleManager.AddClaimAsync(role, newClaim);

                    if (result.Succeeded)
                    {
                        Console.WriteLine($"Claim '{claimValue}' added to role '{roleName}'.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to add claim '{claimValue}' to role '{roleName}'.");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Claim '{claimValue}' already assigned to role '{roleName}'.");
                }
            }
            else
            {
                Console.WriteLine($"Role '{roleName}' not found.");
            }
        }

        public static async Task SeedRoleClaimsAsync(RoleManager<IdentityRole> roleManager)
        {
            await SeedRoleClaimAsync(roleManager, "User", "can_view_dashboard");
            await SeedRoleClaimAsync(roleManager, "Admin", "can_manage_users");
            await SeedRoleClaimAsync(roleManager, "Manager", "can_manage_content");
            await SeedRoleClaimAsync(roleManager, "Employee", "can_access_employee_features");
        }



        private async Task SeedRoleClaims(RoleManager<IdentityRole> roleManager, string roleName)
        {
            // Retrieve the role by its name
            var role = await roleManager.FindByNameAsync(roleName);

            // Check if the role has any claims
            var existingClaims = await roleManager.GetClaimsAsync(role);

            // Add role claims if they don't exist
            if (!existingClaims.Any(c => c.Type == "YourClaimType" && c.Value == "YourClaimValue"))
            {
                // Add the role claims you need for the specific role
                await roleManager.AddClaimAsync(role, new Claim("YourClaimType", "YourClaimValue"));
                Console.WriteLine($"Role claim added for '{roleName}'.");
            }
            else
            {
                Console.WriteLine($"Role claim for '{roleName}' already exists.");
            }

            // Add more role claims as needed
        }

        public async Task CreateOrder(AppDbContext _context)
        {
            if (!_context.Orders.Any())
            {
                // Create Orders
                _context.Orders.AddRange(
                 new Order
                  {
                      Comment = "This order is so good that it should be named 'The Masterpiece'. Can't wait to taste the magic!",
                      Accepted = true,
                      TimeStamp = DateTime.UtcNow,
                      KitchenComment = "This order is so good that it should be named 'The Masterpiece'",
                      Delivered = false
                  },
            new Order
            {
                Comment = "Just placed an order for the most fantastic meal! Cooking this order felt like preparing a meal for royalty.",
                Accepted = false,
                TimeStamp = DateTime.UtcNow,
                KitchenComment = "Cooking this order felt like preparing a meal for royalty",
                Delivered = true
            },
            new Order
            {
                Comment = "Ordered a feast that's so amazing it deserves its own holiday! Can't wait to dig in.",
                Accepted = true,
                TimeStamp = DateTime.UtcNow,
                KitchenComment = "This order is so good that it should be named 'The Masterpiece'",
                Delivered = false
            },
            new Order
            {
                Comment = "Just ordered a meal that's out of this world! If only every day could be this delicious.",
                Accepted = false,
                TimeStamp = DateTime.UtcNow,
                KitchenComment = "Cooking this order felt like preparing a meal for royalty",
                Delivered = true
            },
            new Order
            {
                Comment = "Placed an order for a culinary masterpiece! Brace yourself for a taste bud explosion.",
                Accepted = true,
                TimeStamp = DateTime.UtcNow,
                KitchenComment = "This order is so good that it should be named 'The Masterpiece'",
                Delivered = false
            },
             new Order
             {
                 Comment = "Ordering this meal feels like unlocking a secret level of flavor! Can't wait to savor every bite.",
                 Accepted = false,
                 TimeStamp = DateTime.UtcNow, // Use UTC time
                 KitchenComment = "Cooking this order felt like preparing a meal for royalty",
             }
            );
                await _context.SaveChangesAsync();

                // Create OrderLines
                var orderLine1 = new OrderLine
                {
                    Quantity = 2,
                    ProductID = 1,
                    OrderID = 1,
                };

                var orderLine2 = new OrderLine
                {
                    Quantity = 1,
                    ProductID = 3,
                    OrderID = 2,
                };

                var orderLine3 = new OrderLine
                {
                    Quantity = 3,
                    ProductID = 5,
                    OrderID = 3,
                };

                var orderLine4 = new OrderLine
                {
                    Quantity = 1,
                    ProductID = 2,
                    OrderID = 4,
                };

                var orderLine5 = new OrderLine
                {
                    Quantity = 2,
                    ProductID = 4,
                    OrderID = 5,
                };

                var orderLine6 = new OrderLine
                {
                    Quantity = 1,
                    ProductID = 6,
                    OrderID = 6,
                };

                _context.OrderLines.AddRange(orderLine1, orderLine2, orderLine3, orderLine4, orderLine5, orderLine6);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<string> GetRoleIdAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            // Check if the role already exists
            var role = await roleManager.FindByNameAsync(roleName);

            // If the role doesn't exist, create it
            if (role == null)
            {
                role = new IdentityRole { Name = roleName };
                await roleManager.CreateAsync(role);
            }

            return role.Id;
        }
        private async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // List of users to be seeded
            var users = new List<(string UserName, string NormalizedUserName, string Email, string NormalizedEmail, string Password, bool EmailConfirmed, bool LockoutEnabled)>
    {
    // User data: (UserName, NormalizedUserName, Email, NormalizedEmail, Password, EmailConfirmed, LockoutEnabled)
        ("kim@example.com", "KIM@EXAMPLE.COM", "kim@example.com", "KIM@EXAMPLE.COM", "Password123!", true, false),
        ("julia@example.com", "JULIA@EXAMPLE.COM", "julia@example.com", "JULIA@EXAMPLE.COM", "Password123!", true, false),
        ("joel@example.com", "JOEL@EXAMPLE.COM", "joel@example.com", "JOEL@EXAMPLE.COM", "Password123!", true, false),
        ("simon@example.com", "SIMON@EXAMPLE.COM", "simon@example.com", "SIMON@EXAMPLE.COM", "Password123!", true, false),
        ("peter@example.com", "PETER@EXAMPLE.COM", "peter@example.com", "PETER@EXAMPLE.COM", "Password123!", true, false),
        ("paul@example.com", "PAUL@EXAMPLE.COM", "paul@example.com", "PAUL@EXAMPLE.COM", "Password123!", true, false)
    };

            // Iterate through each user data and seed the user
            foreach (var (userName, normalizedUserName, email, normalizedEmail, password, emailConfirmed, lockoutEnabled) in users)
            {
                // Check if the user already exists
                var user = await userManager.FindByNameAsync(userName);

                // If the user doesn't exist, create and seed the user
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = userName,
                        NormalizedUserName = normalizedUserName,
                        Email = email,
                        NormalizedEmail = normalizedEmail,
                        EmailConfirmed = emailConfirmed,
                        LockoutEnabled = lockoutEnabled
                    };

                    // Create the user and handle errors if any
                    var result = await userManager.CreateAsync(user, password);

                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"Failed to create user '{userName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }



        private async Task AssignUserRoleAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, string userName, string roleName)
        {
            // Get user by name
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                Console.WriteLine($"User '{userName}' not found.");
                return;
            }

            // Get role ID by name
            var roleId = await GetRoleIdAsync(roleManager, roleName);

            // Check if the user is already assigned to the role
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                // Assign the user to the specified role
                await userManager.AddToRoleAsync(user, roleName);
                Console.WriteLine($"User '{userName}' assigned to role '{roleName}'.");
            }
            else
            {
                Console.WriteLine($"User '{userName}' is already assigned to role '{roleName}'.");
            }
        }


        public async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { "Admin", "Manager", "Employee", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    // SeedRoles
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"Role '{role}' created.");

                    // Seed role claims for the newly created role
                    await SeedRoleClaims(roleManager, role);
                }
                else
                {
                    Console.WriteLine($"Role '{role}' already exists.");
                }
            }
        }

        public Dictionary<string, int> RetrieveSubCategoryId(AppDbContext context)
        {
            // Retrieve SubCategory ID
            Dictionary<string, int> subCategoryIds = context.SubCategories.ToDictionary(sub => sub.Name, sub => sub.ID);
            return subCategoryIds;


        }

        public async Task SeedMerch(AppDbContext context)
        {
            context.Merch.AddRange(
                new Models.Merch
                {
                    Size = "S",
                    Color = "White",
                },
                new Models.Merch
                {
                    Size = "M",
                    Color = "White",
                },
                new Models.Merch
                {
                    Size = "L",
                    Color = "White",
                },
                new Models.Merch
                {
                    Size = "S",
                    Color = "Red",
                },
                new Models.Merch
                {
                    Size = "M",
                    Color = "Red",
                },
                new Models.Merch
                {
                    Size = "L",
                    Color = "Red",
                }
                );
        }
        public async Task AllergiesAndDrinks(AppDbContext context)
        {
            context.Drinks.AddRange(

                           new Drink
                           {


                               AlcoholPercentage = 25



                           },
                           new Drink
                           {


                               AlcoholPercentage = 80



                           },
                           new Drink
                           {


                               AlcoholPercentage = 0



                           },
                           new Drink
                           {


                               AlcoholPercentage = 10



                           }




                           );

            context.Foods.AddRange(

                new Food
                {
                    Nuts = false,
                    Lactose = false
                },
                                    new Food
                                    {
                                        Nuts = false,
                                        Lactose = true

                                    }, new Food
                                    {
                                        Nuts = true,
                                        Lactose = true
                                    }, new Food
                                    {
                                        Nuts = true,
                                        Lactose = false
                                    }

                );

        }
        public async Task AssignSubCategoryId(AppDbContext context)

        {
            // RetriveSubCategoryId
            var subCategoryIds = RetrieveSubCategoryId(context);

            var bevrageSubCategory = new SubCategory { Name = "Beverages " };
            var mainCourseSubCategory = new SubCategory { Name = "Main Courses " };
            var dessertSubCategory = new SubCategory { Name = "Desserts " };
            var merchSubCategory = new SubCategory { Name = "Merch " };

            // Assign existing SubCategory IDs
            bevrageSubCategory.ID = subCategoryIds.GetValueOrDefault(bevrageSubCategory.Name);
            mainCourseSubCategory.ID = subCategoryIds.GetValueOrDefault(mainCourseSubCategory.Name);
            dessertSubCategory.ID = subCategoryIds.GetValueOrDefault(dessertSubCategory.Name);
            dessertSubCategory.ID = subCategoryIds.GetValueOrDefault(merchSubCategory.Name);

            // Check if database is already populated
            if (context.SubCategories.Any())
            {

                Console.WriteLine("Database is already populated. Skipping seeding...");


            }
            else
            {
                // Seeding


                context.SubCategories.AddRange(bevrageSubCategory, mainCourseSubCategory, dessertSubCategory, merchSubCategory);
                await context.SaveChangesAsync();  // Save changes to generate IDs


            }

            context.Products.AddRange(
                        new Product
                        {

                            Name = "Vap",
                            Price = 1.99m,
                            Description = "Refreshing carbonated beverage",
                            Image = "soda.png",
                            Available = true,
                            SubCategoryId = bevrageSubCategory.ID,
                            DrinkID = 3,
                            VAT = 12

                        },
                                                new Product
                                                {

                                                    Name = "Beer",
                                                    Price = 1.99m,
                                                    Description = "Arboga, refreshing carbonated beverage",
                                                    Image = "soda.png",
                                                    Available = true,
                                                    SubCategoryId = bevrageSubCategory.ID,
                                                    DrinkID = 4,
                                                    VAT = 25,

                                                },
                                                                        new Product
                                                                        {

                                                                            Name = "Absinth",
                                                                            Price = 1.99m,
                                                                            Description = "Strong spirit",
                                                                            Image = "soda.png",
                                                                            Available = true,
                                                                            SubCategoryId = bevrageSubCategory.ID,
                                                                            DrinkID = 2,
                                                                            VAT = 25,

                                                                        }, new Product
                                                                        {

                                                                            Name = "California Sirah",
                                                                            Price = 1.99m,
                                                                            Description = "Strong spirit",
                                                                            Image = "soda.png",
                                                                            Available = true,
                                                                            SubCategoryId = bevrageSubCategory.ID,
                                                                            DrinkID = 1,
                                                                            VAT = 25,

                                                                        },
                        new Product
                        {

                            Name = "Iced Tea",
                            Price = 1.99m,
                            Description = "Chilled tea served with ice",
                            Image = "iced_tea.jpg",
                            Available = true,
                            SubCategoryId = bevrageSubCategory.ID,
                            DrinkID = 3,
                            VAT = 12,
                        },
                        new Product
                        {

                            Name = "Fruit Smoothie",
                            Price = 4.99m,
                            Description = "Blend of fresh fruits and yogurt",
                            Image = "Blended-fruit-smoothies.jpg",
                            Available = true,
                            SubCategoryId = bevrageSubCategory.ID,
                            DrinkID = 3,
                            VAT = 12,
                        },
                        new Product
                        {

                            Name = "Grilled Chicken Sandwich",
                            Price = 8.99m,
                            Description = "Grilled chicken breast with fresh veggies on a bun",
                            Image = "grilled_chicken_sandwich.jpg",
                            Available = true,
                            SubCategoryId = mainCourseSubCategory.ID,
                            FoodID = 1,
                            VAT = 12
                        },
                        new Product
                        {

                            Name = "Classic Burger",
                            Price = 10.99m,
                            Description = "Juicy beef patty with lettuce, tomato and cheese",
                            Image = "Classic_Burger.jpg",
                            Available = true,
                            SubCategoryId = mainCourseSubCategory.ID,
                            FoodID = 2,
                            VAT = 12
                        },
                        new Product
                        {

                            Name = "Vegetarian Pizza",
                            Price = 10.99m,
                            Description = "Thin-crust pizza with assorted veggies",
                            Image = "Vegetable-Pizza.jpg",
                            Available = true,
                            SubCategoryId = mainCourseSubCategory.ID,
                            FoodID = 2,
                            VAT = 12
                        },
                        new Product
                        {
                            Name = "Grilled Salmon",
                            Price = 12.99m,
                            Description = "Freshly grilled salmon fillet with lemon butter",
                            Image = "grilled_Salmon.jpg",
                            Available = true,
                            SubCategoryId = mainCourseSubCategory.ID,
                            FoodID = 2,
                            VAT = 12
                        },


                        new Product
                        {

                            Name = "Chocolate Brownie Sundae",
                            Price = 5.99m,
                            Description = "Warm chocolate brownie topped with vanilla ice cream and hot fudge",
                            Image = "Fudge_Sunday.jpg",
                            Available = true,
                            SubCategoryId = dessertSubCategory.ID,
                            FoodID = 3,
                            VAT = 12

                        },
                        new Product
                        {

                            Name = "Cheesecake",
                            Price = 6.99m,
                            Description = "Creamy and rich New York - style cheesecake",
                            Image = "CheeseCake.jpg",
                            Available = true,
                            SubCategoryId = dessertSubCategory.ID,
                            FoodID = 3,
                            VAT = 12


                        },
                        new Product
                        {

                            Name = "Fudge Brownie",
                            Price = 4.99m,
                            Description = "Decadent chocolate fudge brownie",
                            Image = "Brownie.jpg",
                            Available = true,
                            SubCategoryId = dessertSubCategory.ID,
                            FoodID = 3,
                            VAT = 12


                        },
                        new Product
                        {

                            Name = "Tiramisu",
                            Price = 8.99m,
                            Description = "Classic Italian dessert with layers of coffee-soaked ladyfingers and mascarpone",
                            Image = "Tiramisu.jpg",
                            Available = true,
                            SubCategoryId = dessertSubCategory.ID,
                            DrinkID = 4,
                            FoodID = 3,
                            VAT = 12,

                        },
                        new Product
                        {

                            Name = "shirt",
                            Price = 99m,
                            Description = "A sick tshirt",
                            Image = "shirt.jpg",
                            Available = true,
                            SubCategoryId = merchSubCategory.ID,
                            VAT = 12,
                            MerchID = 1
                        },
                        new Product
                        {

                            Name = "shirt",
                            Price = 99m,
                            Description = "A sick tshirt",
                            Image = "shirt.jpg",
                            Available = true,
                            SubCategoryId = merchSubCategory.ID,
                            VAT = 12,
                            MerchID = 2
                        },
                        new Product
                        {

                            Name = "shirt",
                            Price = 99m,
                            Description = "A sick tshirt",
                            Image = "shirt.jpg",
                            Available = true,
                            SubCategoryId = merchSubCategory.ID,
                            VAT = 12,
                            MerchID = 3
                        },
                        new Product
                        {

                            Name = "shirt",
                            Price = 99m,
                            Description = "A sick tshirt",
                            Image = "shirtRed.jpg",
                            Available = true,
                            SubCategoryId = merchSubCategory.ID,
                            VAT = 12,
                            MerchID = 4
                        },
                        new Product
                        {

                            Name = "shirt",
                            Price = 99m,
                            Description = "A sick tshirt",
                            Image = "shirtRed.jpg",
                            Available = true,
                            SubCategoryId = merchSubCategory.ID,
                            VAT = 12,
                            MerchID = 5
                        },
                        new Product
                        {

                            Name = "shirt",
                            Price = 99m,
                            Description = "A sick tshirt",
                            Image = "shirtRed.jpg",
                            Available = true,
                            SubCategoryId = merchSubCategory.ID,
                            VAT = 12,
                            MerchID = 6
                        }
                        );

        }

    }
}
