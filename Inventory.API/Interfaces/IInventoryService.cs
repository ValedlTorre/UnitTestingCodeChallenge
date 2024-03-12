using Inventory.API.Models;
using Inventory.API.Models.Request;

namespace Inventory.API.Interfaces
{
    public interface IInventoryService
    {
        public Response<decimal> GetIvaValue();
        public ResponseBase UpdateIva(decimal iva);

        public Response<int> UpdateProductQuantity(ProductMovementRequest request);
        public Response<Product> AddProduct(string name, decimal price, int quantity);
        public ResponseBase RemoveProduct(string name);
        public Response<decimal> UpdateProductPrice(string productName, decimal newPrice);
        public Response<Product> GetProductByName(string name);
        public Response<List<Product>> GetProductList();
        public decimal CalculateProductTotalValue(decimal price, int quantity);
    }
}
