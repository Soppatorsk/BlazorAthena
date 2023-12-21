using Microsoft.EntityFrameworkCore;
using AthenaResturantWebAPI.Data.Context;
using BlazorAthena.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AthenaResturantWebAPI.Controllers
{
    [Authorize]
    public static class OrderEndpoints
    {
        [HttpPost("Order")]
        public static void MapOrderEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Order").WithTags(nameof(Order));

            // Endpoint 1: Get all orders
            group.MapGet("/",  async (AppDbContext db) =>
                await db.Orders.ToListAsync())
                .WithName("GetAllOrders")
                .WithOpenApi();

        //this works in swagger now
        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Order updatedOrder, AppDbContext db) =>
        {
            var existingOrder = await db.Orders
                .FirstOrDefaultAsync(model => model.ID == id);

            if (existingOrder == null)
            {
                return TypedResults.NotFound();
            }

            // Update only the necessary properties
            existingOrder.Comment = updatedOrder.Comment;
            existingOrder.Accepted = updatedOrder.Accepted;
            existingOrder.TimeStamp = updatedOrder.TimeStamp;
            existingOrder.KitchenComment = updatedOrder.KitchenComment;
            existingOrder.Delivered = updatedOrder.Delivered;
            existingOrder.SaleAmount = updatedOrder.SaleAmount;

            var affected = await db.SaveChangesAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
          .WithName("UpdateOrder")
          .WithOpenApi();
          
            // Endpoint 2: Get order by ID
            group.MapGet("/{id}", async Task<Results<Ok<Order>, NotFound>> (int id, AppDbContext db) =>
                await db.Orders.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.ID == id)
                    is Order model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound())
                .WithName("GetOrderById")
                .WithOpenApi();

            // Endpoint 3: Update order
            group.MapPut("/{id}",  async Task<Results<Ok, NotFound>> (int id, Order order, AppDbContext db) =>
            {
                var affected = await db.Orders
                    .Where(model => model.ID == id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(m => m.ID, order.ID)
                        .SetProperty(m => m.Comment, order.Comment)
                        .SetProperty(m => m.Accepted, order.Accepted)
                        .SetProperty(m => m.TimeStamp, order.TimeStamp)
                        .SetProperty(m => m.KitchenComment, order.KitchenComment)
                        .SetProperty(m => m.Delivered, order.Delivered)
                        .SetProperty(m => m.SaleAmount, order.SaleAmount)
                    );
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
                .WithName("UpdateOrder")
                .WithOpenApi();

            // Endpoint 4: Create order
            group.MapPost("/", async (Order order, AppDbContext db) =>
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return TypedResults.Created($"/api/Order/{order.ID}", order);
            })
                .WithName("CreateOrder")
                .WithOpenApi();

            // Endpoint 5: Delete order
            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
            {
                var affected = await db.Orders
                    .Where(model => model.ID == id)
                    .ExecuteDeleteAsync();
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
                .WithName("DeleteOrder")
                .WithOpenApi();
        }
    }
}
