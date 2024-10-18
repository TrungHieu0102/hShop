using Application.Constants;
using Application.DTOs.ProductsDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "", [FromQuery] bool IsDecsending = false)
        {
            var pagedResult = await _productService.GetAllProductsAsync(page, pageSize, search, IsDecsending);
            return Ok(pagedResult);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);

            }
            return Ok(result.Data);
        }
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromForm] CreateUpdateProductDto productDto)
        {
            try
            {
                var result = await _productService.AddProductAsync(productDto);
                if (result == 0)
                {
                    return StatusCode(500, "Failed to create product");
                }
                return StatusCode(201, new { message = "Product created successfully", productId = result });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] CreateUpdateProductDto productDto)
        {
            var result = await _productService.UpdateProductAsync(id, productDto);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            if (!await _productService.DeleteProductAsync(id))
            {
                return BadRequest();
            }
            return NoContent();
        }
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductByCategory(Guid categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool IsDecsending = false)
        {
            var pagedResult = await _productService.GetProductByCategoryAsync(categoryId, page, pageSize, IsDecsending);
            return Ok(pagedResult);
        }
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchProductByName([FromQuery] string name, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool IsDecsending = false)
        {
            var pagedResult = await _productService.SearchProductByNameAsync(name, page, pageSize, IsDecsending);
            return Ok(pagedResult);
        }
    }
}