using Microsoft.EntityFrameworkCore;
using AthenaResturantWebAPI.Data.Context;
using BlazorAthena.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace AthenaResturantWebAPI.Controllers;

public static class SubCategoryEndpoints
{
    public static void MapSubCategoryEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/SubCategory").WithTags(nameof(SubCategory));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.SubCategories.ToListAsync();
        })
        .WithName("GetAllSubCategories")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<SubCategory>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.SubCategories.AsNoTracking()
                .FirstOrDefaultAsync(model => model.ID == id)
                is SubCategory model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetSubCategoryById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, SubCategory subCategory, AppDbContext db) =>
        {
            var affected = await db.SubCategories
                .Where(model => model.ID == id)
                .ExecuteUpdateAsync(setters => setters
                //Updating ID bad ?
                   //.SetProperty(m => m.ID, subCategory.ID)
                    .SetProperty(m => m.Name, subCategory.Name)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateSubCategory")
        .WithOpenApi();

        group.MapPost("/", async (SubCategory subCategory, AppDbContext db) =>
        {
            db.SubCategories.Add(subCategory);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/SubCategory/{subCategory.ID}",subCategory);
        })
        .WithName("CreateSubCategory")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.SubCategories
                .Where(model => model.ID == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteSubCategory")
        .WithOpenApi();
    }
}
