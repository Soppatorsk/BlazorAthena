﻿using Microsoft.EntityFrameworkCore;
using AthenaResturantWebAPI.Data.Context;
using BlazorAthena.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace AthenaResturantWebAPI.Controllers;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Order").WithTags(nameof(Order));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Orders.ToListAsync();
        })
        .WithName("GetAllOrders")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Order>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Orders.AsNoTracking()
                .FirstOrDefaultAsync(model => model.ID == id)
                is Order model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetOrderById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Order order, AppDbContext db) =>
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

        group.MapPost("/", async (Order order, AppDbContext db) =>
        {
            db.Orders.Add(order);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Order/{order.ID}",order);
        })
        .WithName("CreateOrder")
        .WithOpenApi();

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