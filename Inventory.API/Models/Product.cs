namespace Inventory.API.Models
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal? TotalValue { get; set; }
    }
}
