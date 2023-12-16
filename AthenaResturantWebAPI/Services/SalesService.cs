using AthenaResturantWebAPI.Data.Context;
using BlazorAthena.Models;
using Microsoft.EntityFrameworkCore;

public interface ISalesService
{
    IEnumerable<Order> GetSalesHistory(string timeFrame);
}

public class SalesService : ISalesService
{
    private readonly AppDbContext _dbContext;
    
    public SalesService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Order> GetSalesHistory(string timeFrame)
    {
        DateTime startDate;

        // Determine the start date based on the specified time frame
        switch (timeFrame.ToLower())
        {
            case "day":
                startDate = DateTime.Today;
                break;
            case "week":
                startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                break;
            default:
                // Handle custom time frame or default to all records
                startDate = DateTime.MinValue;
                break;
        }

        // Example LINQ query to retrieve sales history based on the start date
        var salesHistory = _dbContext.Orders
            .Include(o => o.OrderLines)
            .Where(o => o.TimeStamp >= startDate)
            .ToList();

        return salesHistory;
    }
}
