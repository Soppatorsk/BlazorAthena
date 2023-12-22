using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class ProductService
{
    private readonly HttpClient httpClient;

    public ProductService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<string>> GetProductListAsync()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<string>>("https://localhost:5127/api/Product");
        }
        catch (Exception ex)
        {
            // Handle the exception (e.g., log it, throw a custom exception)
            Console.Error.WriteLine($"Error fetching data: {ex.Message}");
            return null;
        }
    }
}
