using Application.Constants;
using Application.DTOs.ProductsDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "", [FromQuery] bool IsDecsending = false)
        {
            var pagedResult = await productService.GetAllProductsAsync(page, pageSize, search, IsDecsending);
            if (pagedResult.IsSuccess == false)
            {
                return BadRequest();
            }
            return Ok(pagedResult);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await productService.GetProductByIdAsync(id);
            if (!result.IsSuccess|| result.Data == null)
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
                var result = await productService.AddProductAsync(productDto);
                return !result.IsSuccess ? StatusCode(400, result.Message) : StatusCode(201, new { message = "Product created successfully"});
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
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] CreateUpdateProductDto productDto)
        {
            var result = await productService.UpdateProductWithImagesAsync(id, productDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok("Update successfully");
        }
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var product = await productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            if (!await productService.DeleteProductAsync(id))
            {
                return BadRequest();
            }
            return NoContent();
        }
        [HttpGet("category/{categoryId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductByCategory(Guid categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool IsDecsending = false)
        {
            var pagedResult = await productService.GetProductByCategoryAsync(categoryId, page, pageSize, IsDecsending);
            return Ok(pagedResult);
        }
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchProductByName([FromQuery] string name, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool IsDecsending = false)
        {
            var pagedResult = await productService.SearchProductByNameAsync(name, page, pageSize, IsDecsending);
            if (pagedResult.IsSuccess == false)
            {
                return BadRequest();
            }
            return Ok(pagedResult);
        }
    }
}