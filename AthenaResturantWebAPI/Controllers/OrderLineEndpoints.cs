using Microsoft.EntityFrameworkCore;
using AthenaResturantWebAPI.Data.Context;
using BlazorAthena.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace AthenaResturantWebAPI.Controllers;

public static class OrderLineEndpoints
{
    public static void MapOrderLineEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/OrderLine").WithTags(nameof(OrderLine));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.OrderLines.ToListAsync();
        })
        .WithName("GetAllOrderLines")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<OrderLine>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.OrderLines.AsNoTracking()
                .FirstOrDefaultAsync(model => model.ID == id)
                is OrderLine model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetOrderLineById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, OrderLine orderLine, AppDbContext db) =>
        {
            var affected = await db.OrderLines
                .Where(model => model.ID == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ID, orderLine.ID)
                    .SetProperty(m => m.ProductID, orderLine.ProductID)
                    .SetProperty(m => m.Quantity, orderLine.Quantity)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateOrderLine")
        .WithOpenApi();

        //group.MapPost("/", async (OrderLine orderLine, AppDbContext db) =>
        //{
        //    db.OrderLines.Add(orderLine);
        //    await db.SaveChangesAsync();
        //    return TypedResults.Created($"/api/OrderLine/{orderLine.ID}",orderLine);
        //})
        //.WithName("CreateOrderLine")
        //.WithOpenApi();
        group.MapPost("/", async (OrderLine orderLine, AppDbContext db) =>
        {
            try
            {
                db.OrderLines.Add(orderLine);

                // Save changes to the database
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order line: {ex.Message}");
            }
        })
.WithName("CreateOrderLine")
.WithOpenApi();




        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.OrderLines
                .Where(model => model.ID == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteOrderLine")
        .WithOpenApi();

        //So one can delete order lines based on the OrderID
        group.MapDelete("/DeleteByOrder/{orderId}", async Task<Results<Ok, NotFound>> (int orderId, AppDbContext db) =>
        {
            var affected = await db.OrderLines
                .Where(model => model.OrderID == orderId)
                .ExecuteDeleteAsync();
            return affected >= 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteOrderLinesByOrder")
        .WithOpenApi();
    }


}
