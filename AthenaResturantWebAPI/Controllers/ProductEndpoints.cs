using Microsoft.EntityFrameworkCore;
using AthenaResturantWebAPI.Data.Context;
using BlazorAthena.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Authorization;
namespace AthenaResturantWebAPI.Controllers;

public static class ProductEndpoints
{
    public static void MapProductEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Product").WithTags(nameof(Product));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Products.ToListAsync();
        })
        .WithName("GetAllProducts")
        .WithOpenApi();

        group.MapGet("/{id}",[Authorize(Roles = "Moderator")] async Task<Results<Ok<Product>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Products.AsNoTracking()
                .FirstOrDefaultAsync(model => model.ID == id)
                is Product model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetProductById")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Product product, AppDbContext db) =>
        {
            var affected = await db.Products
                .Where(model => model.ID == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ID, product.ID)
                    .SetProperty(m => m.Name, product.Name)
                    .SetProperty(m => m.Price, product.Price)
                    .SetProperty(m => m.Description, product.Description)
                    .SetProperty(m => m.Image, product.Image)
                    .SetProperty(m => m.DrinkID, product.DrinkID)
                    .SetProperty(m => m.FoodID, product.FoodID)
                    .SetProperty(m => m.Available, product.Available)
                    .SetProperty(m => m.SubCategoryId, product.SubCategoryId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateProduct")
        .WithOpenApi();

        group.MapPost("/", async (Product product, AppDbContext db) =>
        {
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Product/{product.ID}",product);
        })
        .WithName("CreateProduct")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Products
                .Where(model => model.ID == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteProduct")
        .WithOpenApi();
    }
}
