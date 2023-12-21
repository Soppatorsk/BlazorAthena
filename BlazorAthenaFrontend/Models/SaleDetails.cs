namespace BlazorAthenaFrontend.Models
{
    public class SaleDetails
    {
        public Order Order { get; set; }
        public OrderLine OrderLine { get; set; }
        public Product Product { get; set; }
    }
}
