using BlazorAthenaFrontend.Data;
using BlazorAthenaFrontend.Data.Identity;
using BlazorAthenaFrontend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorAthenaFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("Defaultconnection") ?? throw new InvalidOperationException("Connection string 'Defaultconnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddJwtBearer(options =>
            {
                options.Audience = "https://localhost:7088"; // Audience should match your Web API
                options.Authority = "https://localhost:7235"; // Authority is the URL of your Identity Server (Blazor Server)
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
                options.AddPolicy("EmployeePolicy", policy => policy.RequireRole("Employee"));
            });

            // Add CORS policy
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

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddSingleton<SubCategoryService>();
            builder.Services.AddScoped<JwtDecoderService>();
            builder.Services.AddSingleton<ImageService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddSingleton<TokenService>();
            //builder.Services.AddScoped<TokenService>();

            builder.Services.AddScoped<CartService>();
            builder.Services.AddSingleton<CurrentUserService>();


            builder.Services.AddHttpClient<ProductService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7235/");
            });
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddHttpClient<UserService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7088/");
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Use CORS policy
            app.UseCors("AllowAnyOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
