using Inventory.API.Interfaces;
using Inventory.API.Models;
using Inventory.API.Models.Request;
using Inventory.API.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Tests
{
    public class ProductControllerTests
    {
        private readonly InventoryService _InventoryActions;
        public ProductControllerTests()
        {
            _InventoryActions = new InventoryService();
        }

        [Fact]
        public void GetIva_ShouldReturnDefaultIvaConfiguration()
        {
            // Arrange
            decimal initialIvaValue = 16;

            // Act
            var response = _InventoryActions.GetIvaValue();

            // Assert
            Assert.True(response.Success);
            Assert.Equal(initialIvaValue, response.Data);
        }

        [Theory]
        [InlineData(18, 18, true)]
        [InlineData(21, 21, true)]
        [InlineData(-5, 16, false)] // Si el valor es negativo, no se actualiza y regresa el valor último valor del iva (en este caso el valor default) 
        public void UpdateIva_UpdatesIvaPercentage(decimal newIva, decimal expected, bool expectedSuccess)
        {
            // Act 
            var response = _InventoryActions.UpdateIva(newIva);

            // Assert 
            Assert.Equal(expectedSuccess, response.Success);
            Assert.Equal(expected, _InventoryActions.ivaValue);

        }

        [Theory]
        [InlineData("Manzana", 15, InventaryMovement.In, 25)]
        [InlineData("Pera", 4, InventaryMovement.In, 10)]
        [InlineData("Mango", 20, InventaryMovement.Out, 15)]
        public void UpdateProductQuantity_ShouldAddOrReduceProductQuantity(string productName, int quantity, InventaryMovement movement, int expected)
        {
            // Arrange
            ProductMovementRequest request = new ProductMovementRequest()
            {
                Name = productName,
                Quantity = quantity,
                MovementType = movement
            };

            List<Product> products = new List<Product>()
            {
                new Product() { Name = "Manzana", Price = (decimal) 29.28, Quantity = 10 },
                new Product() { Name = "Pera", Price = (decimal) 32.34, Quantity = 6 },
                new Product() { Name = "Mango", Price = (decimal) 45.27, Quantity = 35 },
            };

            foreach (Product product in products)
            {
                _InventoryActions.AddProduct(product.Name, product.Price, product.Quantity);
            }

            // Act
            Response<int> response = _InventoryActions.UpdateProductQuantity(request);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(expected, response.Data);
        }

        [Fact]
        public void UpdateProductQuantity_WhenRegisteringOutMovement_ShouldNotUpdateWhenQuantityIsGreaterThanTheOneAvailable()
        {
            // Arrange
            int expectedQuantity = 10;
            string expectedMessage = "La cantidad a sacar del inventario excede la cantidad disponible";

            ProductMovementRequest request = new ProductMovementRequest()
            {
                Name = "Manzana",
                Quantity = 20,
                MovementType = InventaryMovement.Out
            };

            _InventoryActions.AddProduct("Manzana", (decimal)29.28, 10);

            // Act
            Response<int> response = _InventoryActions.UpdateProductQuantity(request);

            // Assert
            Assert.False(response.Success);
            Assert.Equal(expectedQuantity, response.Data);
            Assert.Equal(expectedMessage, response.Message);
        }

        [Theory]
        [InlineData("Manzana", 29.92, 10)]
        [InlineData("Manzana", 29.92, 0)]
        [InlineData("Manzana", 0, 10)]
        public void SaveProduct_ShouldAddNewObjectToProductList(string name, decimal price, int quantity)
        {
            // Act 
            Response<Product> response = _InventoryActions.AddProduct(name, price, quantity);

            // Assert 
            Assert.True(response.Success);
            Assert.Single(_InventoryActions.products);
            Assert.Equal(_InventoryActions.products[0], response.Data);
        }

        [Theory]
        [InlineData("", 128, 10)] // Si el nombre esta vacio 
        [InlineData("Manzana", -128, 23)] // Si el precio es menor a 0 
        [InlineData("Manzana", 128, -10)] // Si la cantidad es menor a 0 
        public void SaveProduct_ShouldNotAddProductIfSomeAttributeIsInvalid(string name, decimal price, int quantity)
        {
            // Act 
            Response<Product> response = _InventoryActions.AddProduct(name, price, quantity);

            // Assert 
            Assert.False(response.Success);
            Assert.Null(response.Data);
            Assert.Empty(_InventoryActions.products);
        }

        [Theory]
        [InlineData("Manzana", true, 2)]
        [InlineData("Kiwuii", false, 3)]
        public void DeleteProduct_ShouldRemoveItemFromProductList(string name, bool expectedResult, int expectedItemsInProductList)
        {
            // Arrange 
            List<Product> products = new List<Product>()
            {
                new Product() { Name = "Manzana", Price = (decimal) 29.28, Quantity = 10 },
                new Product() { Name = "Pera", Price = (decimal) 32.34, Quantity = 6 },
                new Product() { Name = "Mango", Price = (decimal) 45.27, Quantity = 4 },
            };

            foreach (Product product in products)
            {
                _InventoryActions.AddProduct(product.Name, product.Price, product.Quantity);
            }

            // Act 
            ResponseBase response = _InventoryActions.RemoveProduct(name);

            // Assert 
            Assert.Equal(expectedResult, response.Success);
            Assert.Equal(expectedItemsInProductList, _InventoryActions.products.Count);
        }

        [Theory]
        [InlineData("Kiwii", 38.29, null, false, "Producto no encontrado")] // Si el producto no se encuentra en la lista, no hay valor para mostrar 
        [InlineData("Mango", -92.92, 45.27, false, "El precio no puede ser menor a 0")] // Si el nuevo precio es negativo, no se actualiza y regresa el valor anterior 
        [InlineData("Pera", 38.92, 38.92, true, "Datos actualizados correctamente")] // Actualización correcta 
        public void UpdatePrice_ShouldUpdateThePriceOfAProduct(string productName, decimal newPrice, decimal expectedPrice, bool expectedSuccess, string expectedMessage)
        {
            // Arrange 
            List<Product> products = new List<Product>()
            {
                new Product() { Name = "Manzana", Price = (decimal) 29.28, Quantity = 10 },
                new Product() { Name = "Pera", Price = (decimal) 32.34, Quantity = 6 },
                new Product() { Name = "Mango", Price = (decimal) 45.27, Quantity = 4 },
            };

            foreach (Product product in products)
            {
                _InventoryActions.AddProduct(product.Name, product.Price, product.Quantity);
            }

            // Act 
            Response<decimal> repsonse = _InventoryActions.UpdateProductPrice(productName, newPrice);

            // Assert 
            Assert.Equal(expectedSuccess, repsonse.Success);
            Assert.Equal(expectedMessage, repsonse.Message);
            Assert.Equal(expectedPrice, repsonse.Data);
        }

        [Fact]
        public void GetProduct_ShouldReturnObjectOfProduct()
        {
            // Arrange 
            List<Product> expected = new List<Product>()
            {
                new Product() { Name = "Manzana", Price = (decimal) 29.28, Quantity = 10 },
                new Product() { Name = "Pera", Price = (decimal) 32.34, Quantity = 6 },
                new Product() { Name = "Mango", Price = (decimal) 45.27, Quantity = 4 },
            };

            foreach (Product product in expected)
            {
                _InventoryActions.AddProduct(product.Name, product.Price, product.Quantity);
            }

            // Act 
            Response<Product> response = _InventoryActions.GetProductByName("Manzana");

            // Assert 
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
        }

        [Fact]
        public void GetProduct_ShouldReturnNullWhenProductCannotBeFound()
        {
            // Arrange 
            List<Product> expected = new List<Product>()
            {
                new Product() { Name = "Manzana", Price = (decimal) 29.28, Quantity = 10 },
                new Product() { Name = "Pera", Price = (decimal) 32.34, Quantity = 6 },
                new Product() { Name = "Mango", Price = (decimal) 45.27, Quantity = 4 },
            };

            foreach (Product product in expected)
            {
                _InventoryActions.AddProduct(product.Name, product.Price, product.Quantity);
            }

            // Act 
            Response<Product> response = _InventoryActions.GetProductByName("Kiwii");

            // Assert 
            Assert.False(response.Success);
            Assert.Null(response.Data);
        }

        [Fact]
        public void GetAllProducts_ShouldReturnTheListOfProducts()
        {
            // Arrange 
            string expectedMessage = "Datos recuperados correctamente";
            List<Product> expected = new List<Product>()
            {
                new Product() { Name = "Manzana", Price = (decimal) 29.28, Quantity = 10 },
                new Product() { Name = "Pera", Price = (decimal) 32.34, Quantity = 6 },
                new Product() { Name = "Mango", Price = (decimal) 45.27, Quantity = 4 },
            };

            foreach (Product product in expected)
            {
                _InventoryActions.AddProduct(product.Name, product.Price, product.Quantity);
            }

            // Act 
            Response<List<Product>> response = _InventoryActions.GetProductList();

            // Assert 
            Assert.Equal(expectedMessage, response.Message);
            Assert.NotEmpty(response.Data);
            Assert.Equal(3, response.Data.Count);
        }

        [Fact]
        public void GetAllProducts_ShouldNotReturnListWhenThereIsNoProductsInInventory()
        {
            // Arrange
            string expectedMessage = "Inventario sin productos";

            // Act 
            Response<List<Product>> response = _InventoryActions.GetProductList();

            // Assert 
            Assert.Equal(expectedMessage, response.Message);
            Assert.Null(response.Data);
        }

        [Theory]
        [InlineData("Manzana", 29.28, 10, 16, 339.648)]
        [InlineData("Manzana", 29.28, 10, 21, 354.288)]
        [InlineData("Manzana", 0, 10, 16, 0)]
        [InlineData("Manzana", 29.28, 0, 21, 0)]
        public void GetAllProducts_ShouldCalculateProductTotalValueWithIva(string name, decimal price, int quantity, decimal iva, decimal expected)
        {
            // Arrange 
            _InventoryActions.AddProduct(name, price, quantity);
            _InventoryActions.UpdateIva(iva);

            // Act 
            Response<List<Product>> response = _InventoryActions.GetProductList();

            // Assert 
            Assert.Equal(expected, response.Data[0].TotalValue);
        }
    }
}
