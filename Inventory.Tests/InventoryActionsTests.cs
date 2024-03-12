using Inventory.API.Models;
using Inventory.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Tests
{
    public class InventoryActionsTests
    {
        private readonly InventoryService _actions;

        public InventoryActionsTests()
        {
            _actions = new InventoryService();
        }

        [Theory]
        [InlineData(35.39, 10, 410.5240)]
        [InlineData(25.78, 15, 448.5720)]
        [InlineData(85.49, 2, 198.3368)]
        public void CalculateProductTotalValue_ShouldReturnTotalValueOfProduct(decimal price, int quantity, decimal expected)
        {
            // Act 
            decimal totalWithIva = _actions.CalculateProductTotalValue(price, quantity);

            // Assert 
            Assert.Equal(expected, totalWithIva);
        }
    }
}
