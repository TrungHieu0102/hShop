using Application.Constants;
using Application.DTOs.ProductsDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using static Core.SeedWorks.Constants.Permissions;

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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "", [FromQuery] bool IsDecsending = false)
        {
            var pagedResult = await _productService.GetAllProductsAsync(page, pageSize, search, IsDecsending);
            return Ok(pagedResult);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.Admin)]

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
        [Authorize(Products.Create)]
        public async Task<ActionResult> AddProduct([FromBody] CreateUpdateProductDto productDto)
        {

            try
            {
                var resutl = await _productService.AddProductAsync(productDto);
                if (resutl == 0)
                {
                    return StatusCode(500, "Failed to create product");
                }
                return Created();
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
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] CreateUpdateProductDto productDto)
        {
            var result = await _productService.UpdateProductAsync(id, productDto);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok();
        }
        [Authorize(Roles = RoleConstants.Admin)]
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