using Inventory.API.Interfaces;
using Inventory.API.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IInventoryService _InventoryService;

        public ProductController(IInventoryService inventoryActions)
        {
            _InventoryService = inventoryActions;
        }

        #region Iva

        /// <summary>
        /// Recupera el valor actual del IVA
        /// </summary>
        [HttpGet("IVA")]
        public ActionResult GetIva()
        {
            try
            {
                var response = _InventoryService.GetIvaValue();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza el valor de la variable IVA
        /// </summary>
        [HttpPut("IVA")]
        public ActionResult UpdateIva([FromBody] decimal iva)
        {
            try
            {
                if (iva > 0) return BadRequest("El valor del IVA no puede ser negativo");

                var response = _InventoryService.UpdateIva(iva);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Products

        /// <summary>
        /// Identificando el tipo de movimiento, actualiza la cantidad del producto en el inventario
        /// </summary>
        [HttpPatch("Movimiento")]
        public ActionResult AddProductMovement([FromBody] ProductMovementRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name)) return BadRequest("Nombre del producto requerido");

                var response = _InventoryService.UpdateProductQuantity(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Registra un nuevo producto en el inventario
        /// </summary>
        [HttpPost]
        public ActionResult SaveProduct([FromBody] ProductRequest request)
        {
            try
            {
                var response = _InventoryService.AddProduct(request.Name, request.Price, request.Quantity);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina un producto del inventario
        /// </summary>
        [HttpDelete("{name}")]
        public ActionResult DeleteProduct([FromRoute] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return BadRequest("Nombre del producto requerido.");

                var response = _InventoryService.RemoveProduct(name);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza el precio unitario de un producto en el inventario
        /// </summary>
        [HttpPatch]
        public ActionResult UpdatePrice([FromBody] ProductPriceRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name)) return BadRequest("Nombre del producto requerido");

                var response = _InventoryService.UpdateProductPrice(request.Name, request.Price);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Recupera la información de un producto específico registrado en el inventario
        /// </summary>
        [HttpGet("{name}")]
        public ActionResult GetProduct([FromRoute] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return BadRequest("El nombre del producto está vacío");

                var response = _InventoryService.GetProductByName(name);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Recupera la lista completa de los productos registrados en el inventario
        /// </summary>
        [HttpGet("All")]
        public ActionResult GetAllProducts()
        {
            try
            {
                var response = _InventoryService.GetProductList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
