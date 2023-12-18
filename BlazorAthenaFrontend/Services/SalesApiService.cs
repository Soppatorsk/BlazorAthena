using BlazorAthenaFrontend.Models;
using BlazorAthenaFrontend.Services;
using Newtonsoft.Json.Linq;
public class SalesApiService
{
    private readonly HttpClient _httpClient;

    public SalesApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SalesData>> GetSalesHistory(string timeFrame)
    {

        try
        {
            // Make the API call
            return await _httpClient.GetFromJsonAsync<List<SalesData>>($"https://localhost:7088/api/sales/history?timeFrame={timeFrame}");
        }
        catch (Exception ex)
        {
            // Handle the exception (e.g., log it or show an error message)
            Console.WriteLine($"Error fetching sales history: {ex.Message}");
            return null; // Or throw an exception, depending on your error handling strategy
        }
    }
}
