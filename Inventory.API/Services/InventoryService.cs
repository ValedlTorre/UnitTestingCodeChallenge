using Inventory.API.Interfaces;
using Inventory.API.Models;
using Inventory.API.Models.Request;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Inventory.API.Services
{
    public class InventoryService : IInventoryService
    {
        public List<Product> products { get; private set; } = new List<Product>();
        public decimal ivaValue { get; private set; } = 16;

        public Response<decimal> GetIvaValue()
        {
            var response = new Response<decimal>()
            {
                Success = true,
                Message = "Información recuperada correctamente",
                Data = ivaValue
            };

            return response;
        }

        public ResponseBase UpdateIva(decimal iva)
        {
            var response = new ResponseBase();

            if (iva < 0)
            {
                response.Success = false;
                response.Message = "Error al recuperar información";

                return response;
            }

            ivaValue = iva;
            response.Success = true;
            response.Message = "Información actualizada correctamente";

            return response;
        }

        public Response<int> UpdateProductQuantity(ProductMovementRequest request)
        {
            Response<int> response = new Response<int>();

            Product product = products.Find(x => x.Name.ToLower() == request.Name.ToLower());

            if (product == null)
            {
                response.Success = false;
                response.Message = "No se encontró el producto a actualizar";

                return response;
            }

            if (request.Quantity < 0)
            {
                response.Success = false;
                response.Message = "Cantidad a actualizar inválida";

                return response;
            }

            // Si se está registrando una salida, y la cantidad es mayor a la que se tiene registrada en el inventario
            // No hacer nada, porque no puede quedar en negativos
            if (request.MovementType == InventaryMovement.Out && request.Quantity > product.Quantity)
            {
                response.Success = false;
                response.Message = "La cantidad a sacar del inventario excede la cantidad disponible";
                response.Data = product.Quantity;

                return response;
            }

            if (request.MovementType == InventaryMovement.In) product.Quantity += request.Quantity;
            else product.Quantity -= request.Quantity;

            response.Data = product.Quantity;
            response.Success = true;
            response.Message = "Datos actualizados correctamente";

            return response;
        }

        public Response<Product> AddProduct(string name, decimal price, int quantity)
        {
            var response = new Response<Product>();

            if (string.IsNullOrEmpty(name) || price < 0 || quantity < 0)
            {
                response.Success = false;
                response.Message = "Error al guardar la información";
                response.Data = null;

                return response;
            }
            
            Product product = new Product
            {
                Name = name,
                Price = price,
                Quantity = quantity
            };

            products.Add(product);

            response.Success = true;
            response.Message = "Datos guardados correctamente";
            response.Data = product;

            return response;
        }

        public ResponseBase RemoveProduct(string name)
        {
            var response = new ResponseBase();

            Product product = products.Find(x => x.Name == name);

            if (product == null)
            {
                response.Success = false;
                response.Message = "Error al eliminar datos";

                return response;
            }

            products.Remove(product);

            response.Success = true;
            response.Message = "Datos eliminados correctamente";

            return response;
        }

        public Response<decimal> UpdateProductPrice(string productName, decimal newPrice)
        {
            var response = new Response<decimal>()
            {
                Success = false,
                Message = string.Empty,
                Data = 0,
            };

            Product product = products.Find(x => x.Name == productName);

            if (product == null)
            {
                response.Message = "Producto no encontrado";
                return response;
            }

            if (newPrice < 0)
            {
                response.Data = product.Price;
                response.Message = "El precio no puede ser menor a 0";
                return response;
            }

            product.Price = newPrice;

            response.Success = true;
            response.Message = "Datos actualizados correctamente";
            response.Data = product.Price;

            return response;
        }

        public Response<Product> GetProductByName(string name)
        {
            var response = new Response<Product>();

            List<Product> productList = GetProductList().Data;

            Product product = productList.Find(x => x.Name.Equals(name));

            if (product == null)
            {
                response.Success = false;
                response.Message = "Producto no encontrado";

                return response;
            }

            response.Success = true;
            response.Message = "Información recuperada correctamente";
            response.Data = product;

            return response;
        }

        public Response<List<Product>> GetProductList()
        {
            var response = new Response<List<Product>>()
            {
                Success = true
            };

            if (products.Count == 0)
            {
                response.Message = "Inventario sin productos";
                return response;
            }

            foreach (var product in products)
            {
                decimal productTotalWithIva = CalculateProductTotalValue(product.Price, product.Quantity);
                product.TotalValue = productTotalWithIva;
            }

            response.Success = true;
            response.Message = "Datos recuperados correctamente";
            response.Data = products;

            return response;
        }

        public decimal CalculateProductTotalValue(decimal price, int quantity)
        {
            decimal total = price * quantity;
            decimal productIva = total * (ivaValue / 100);

            total += productIva;

            return total;
        }
    }
}
