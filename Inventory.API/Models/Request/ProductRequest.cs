namespace Inventory.API.Models.Request
{
    public class ProductRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductPriceRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductMovementRequest
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public InventaryMovement MovementType { get; set; }
    }
}
