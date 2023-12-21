using AthenaResturantWebAPI.Data.Context;
using Microsoft.EntityFrameworkCore;
using AthenaResturantWebAPI.Controllers;
using AthenaResturantWebAPI.Services;
using Microsoft.AspNetCore.Identity;
using System;
using AthenaResturantWebAPI.Data.AppUser;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Components.Server;

namespace AthenaResturantWebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers();
            // Configure JWT authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var jwtSecretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; 
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSecretKey),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                };
            });
      
            builder.Services.AddAuthorization();
            // JwtService
            builder.Services.AddScoped<JwtService>();
            // Adding SalesService
            builder.Services.AddScoped<ISalesService, SalesService>();


            // AddScoped for ProductServices
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddHttpClient<ProductService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7088/");
            });
            // AddHttpClient registration
            builder.Services.AddHttpClient();


            // CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register Identity services
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<IdentityRole>()  // Add this line to enable roles
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


            // Add authentication state provider
            builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

            // Add IHttpContextAccessor
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();



            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                // Obtain the necessary services
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(); 
                // Call the SeedData method
                var seedDataService = new GeneralServices(appDbContext,userManager, roleManager);
                await seedDataService.SeedData(appDbContext, userManager, roleManager);

            }




            var app = builder.Build();
            //app.MapPost("/product", [Authorize] () => "Simon is King").RequireAuthorization();
            app.MapGet("/product", [Authorize] () => "Manager").RequireAuthorization();
            //app.MapPut("/product", [Authorize] () => "Simon is Master").RequireAuthorization();
            //app.MapDelete("/product", [Authorize] () => "Simon is Senpai").RequireAuthorization();
            // Configure he HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // CORS middleware
            app.UseCors();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllers();

            app.MapSubCategoryEndpoints();

            app.MapProductEndpoints();

            app.MapOrderEndpoints();

            app.MapOrderLineEndpoints();
                        
            
            app.Run();
        }
    }
}
