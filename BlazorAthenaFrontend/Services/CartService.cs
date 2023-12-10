namespace BlazorAthenaFrontend.Services
{
    using BlazorAthenaFrontend.Models;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CartService
    {
        private readonly HttpClient httpClient;

        public CartService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public List<Product> SelectedProducts { get; set; } = new List<Product>();
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

        public async Task AddProductToCartAsync(Product chosenProduct)
        {
            SelectedProducts.Add(chosenProduct);
        }

        public void AddOrderLine(int productId, int quantity)
        {
            var orderLine = new OrderLine
            {
                ProductID = productId,
                Quantity = quantity
            };

            OrderLines.Add(orderLine);
        }

        public void ClearCart()
        {
            SelectedProducts.Clear();
            OrderLines.Clear();
        }

        // Remove or adjust the FetchProductAsync method as it is not being used
        private async Task<Product> FetchProductAsync(int productId)
        {
            try
            {
                // Make an API request to get product information based on productId
                var product = await httpClient.GetFromJsonAsync<Product>($"https://localhost:7088/api/Product/{productId}");
                return product;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error fetching product: {ex.Message}");
                throw;
            }
        }
    }
}
