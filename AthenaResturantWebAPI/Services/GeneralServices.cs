﻿using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BlazorAthena.Models;
using AthenaResturantWebAPI.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using AthenaResturantWebAPI.Data.AppUser;

namespace AthenaResturantWebAPI.Services
{
    public class GeneralServices
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
    
        public async Task SeedData(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            try
            {
             
              
                
                if (!context.Products.Any())
                {
                    // Seed Roles 
                    await SeedRoles(roleManager);
                    _context.SaveChanges();
                    // Seed Drinks and Foods
                    await AllergiesAndDrinks(context);
                    // Seed Products
                    await AssignSubCategoryId(context);
                    _context.SaveChanges();
                    // Seed Users
                    await SeedUsersAsync(userManager);
                    _context.SaveChanges();
                    await AssignUserRoleAsync(userManager, roleManager, "Kim", "Employee");
                    await AssignUserRoleAsync(userManager, roleManager, "Julia", "Manager");
                    await AssignUserRoleAsync(userManager, roleManager, "Joel", "Admin");
                    await AssignUserRoleAsync(userManager, roleManager, "Simon", "Admin");
                    await AssignUserRoleAsync(userManager, roleManager, "Peter", "Manager");
                    await AssignUserRoleAsync(userManager, roleManager, "Pual", "User");
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

        public async Task CreateOrder(AppDbContext _context)
        {
            if (!_context.Orders.Any())
            {
                // Create OrderLines
                var orderLine1 = new OrderLine
                {
                    Quantity = 2,
                    ProductID = 1,
                };

                var orderLine2 = new OrderLine
                {
                    Quantity = 1,
                    ProductID = 3,
                };

                var orderLine3 = new OrderLine
                {
                    Quantity = 3,
                    ProductID = 5,
                };

                var orderLine4 = new OrderLine
                {
                    Quantity = 1,
                    ProductID = 2,
                };

                var orderLine5 = new OrderLine
                {
                    Quantity = 2,
                    ProductID = 4,
                };
                var orderLine6 = new OrderLine
                {
                    Quantity = 1,
                    ProductID = 6,
                };

                _context.OrderLines.AddRange(orderLine1, orderLine2, orderLine3, orderLine4, orderLine5, orderLine6);
                await _context.SaveChangesAsync();

                // Create Orders
                _context.Orders.AddRange(
                  new Order
                  {
                      OrderLineID = orderLine1.ID,
                      Comment = "This order is so good that it should be named 'The Masterpiece'. Can't wait to taste the magic!",
                      Accepted = true,
                      TimeStamp = DateTime.UtcNow,
                      KitchenComment = "This order is so good that it should be named 'The Masterpiece'",
                      Delivered = false
                  },
            new Order
            {
                OrderLineID = orderLine2.ID,
                Comment = "Just placed an order for the most fantastic meal! Cooking this order felt like preparing a meal for royalty.",
                Accepted = false,
                TimeStamp = DateTime.UtcNow,
                KitchenComment = "Cooking this order felt like preparing a meal for royalty",
                Delivered = true
            },
            new Order
            {
                OrderLineID = orderLine3.ID,
                Comment = "Ordered a feast that's so amazing it deserves its own holiday! Can't wait to dig in.",
                Accepted = true,
                TimeStamp = DateTime.UtcNow,
                KitchenComment = "This order is so good that it should be named 'The Masterpiece'",
                Delivered = false
            },
            new Order
            {
                OrderLineID = orderLine4.ID,
                Comment = "Just ordered a meal that's out of this world! If only every day could be this delicious.",
                Accepted = false,
                TimeStamp = DateTime.UtcNow,
                KitchenComment = "Cooking this order felt like preparing a meal for royalty",
                Delivered = true
            },
            new Order
            {
                OrderLineID = orderLine5.ID,
                Comment = "Placed an order for a culinary masterpiece! Brace yourself for a taste bud explosion.",
                Accepted = true,
                TimeStamp = DateTime.UtcNow,
                KitchenComment = "This order is so good that it should be named 'The Masterpiece'",
                Delivered = false
            },
             new Order
             {
                 OrderLineID = orderLine6.ID,
                 Comment = "Ordering this meal feels like unlocking a secret level of flavor! Can't wait to savor every bite.",
                 Accepted = false,
                 TimeStamp = DateTime.UtcNow, // Use UTC time
                 KitchenComment = "Cooking this order felt like preparing a meal for royalty",
             }

            );

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
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"Role '{role}' created.");
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

            // Assign existing SubCategory IDs
            bevrageSubCategory.ID = subCategoryIds.GetValueOrDefault(bevrageSubCategory.Name);
            mainCourseSubCategory.ID = subCategoryIds.GetValueOrDefault(mainCourseSubCategory.Name);
            dessertSubCategory.ID = subCategoryIds.GetValueOrDefault(dessertSubCategory.Name);

            // Check if database is already populated
            if (context.SubCategories.Any())
            {

                Console.WriteLine("Database is already populated. Skipping seeding...");


            }
            else
            {
                // Seeding


                context.SubCategories.AddRange(bevrageSubCategory, mainCourseSubCategory, dessertSubCategory);
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

                                                                        }, new Product
                                                                        {

                                                                            Name = "California Sirah",
                                                                            Price = 1.99m,
                                                                            Description = "Strong spirit",
                                                                            Image = "soda.png",
                                                                            Available = true,
                                                                            SubCategoryId = bevrageSubCategory.ID,
                                                                            DrinkID = 1,

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
                        },
                        new Product
                        {

                            Name = "Grilled Chicken Sandwich",
                            Price = 8.99m,
                            Description = "Grilled chicken breast with fresh veggies on a bun",
                            Image = "grilled_chicken_sandwich.jpg",
                            Available = true,
                            SubCategoryId = mainCourseSubCategory.ID,
                            FoodID = 1
                        },
                        new Product
                        {

                            Name = "Classic Burger",
                            Price = 10.99m,
                            Description = "Juicy beef patty with lettuce, tomato and cheese",
                            Image = "Classic_Burger.jpg",
                            Available = true,
                            SubCategoryId = mainCourseSubCategory.ID,
                            FoodID = 2
                        },
                        new Product
                        {

                            Name = "Vegetarian Pizza",
                            Price = 10.99m,
                            Description = "Thin-crust pizza with assorted veggies",
                            Image = "Vegetable-Pizza.jpg",
                            Available = true,
                            SubCategoryId = mainCourseSubCategory.ID,
                            FoodID = 2
                        },
                        new Product
                        {
                            Name = "Grilled Salmon",
                            Price = 12.99m,
                            Description = "Freshly grilled salmon fillet with lemon butter",
                            Image = "grilled_Salmon.jpg",
                            Available = true,
                            SubCategoryId = mainCourseSubCategory.ID,
                            FoodID = 2
                        },


                        new Product
                        {

                            Name = "Chocolate Brownie Sundae",
                            Price = 5.99m,
                            Description = "Warm chocolate brownie topped with vanilla ice cream and hot fudge",
                            Image = "Fudge_Sunday.jpg",
                            Available = true,
                            SubCategoryId = dessertSubCategory.ID,
                            FoodID = 3

                        },
                        new Product
                        {

                            Name = "Cheesecake",
                            Price = 6.99m,
                            Description = "Creamy and rich New York - style cheesecake",
                            Image = "CheeseCake.jpg",
                            Available = true,
                            SubCategoryId = dessertSubCategory.ID,
                            FoodID = 3


                        },
                        new Product
                        {

                            Name = "Fudge Brownie",
                            Price = 4.99m,
                            Description = "Decadent chocolate fudge brownie",
                            Image = "Brownie.jpg",
                            Available = true,
                            SubCategoryId = dessertSubCategory.ID,
                            FoodID = 3


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
                            FoodID = 3

                        }

                        );

        }



    }
}
