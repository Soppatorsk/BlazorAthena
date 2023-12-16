namespace BlazorAthenaFrontend.Services
{
    using BlazorAthenaFrontend.Models;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using BlazorAthenaFrontend.Pages;
  

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

            //also add these products to orderline so a new list?
        }

        public void AddOrderLine(int addedOrderId, List<Product> SelectedProducts) //Kolla på detta. Den ska ju ta quantity samt ALLA enskilda produkter (bara samma en gång)
        {
            var groupedProducts = SelectedProducts
            .GroupBy(p => p.ID)
            .Select(group => new
            {
                ProductID = group.Key,
                Quantity = group.Count()
            });

            foreach (var g in groupedProducts)
            {
                var orderLine = new OrderLine
                {
                    ProductID = g.ProductID,
                    Quantity = g.Quantity,
                    OrderID = addedOrderId,
                };
                OrderLines.Add(orderLine);
            }
        }

        public int ClearCart()
        {
            SelectedProducts.Clear();
            OrderLines.Clear();
            return 0; //clearing total amount
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
