using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductAppService _productService;

    public ProductsController(IProductAppService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult> AddProduct(ProductDto productDto)
    {
        await _productService.AddProductAsync(productDto);
        return CreatedAtAction(nameof(GetProductById), new { id = productDto.Id }, productDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(Guid id, ProductDto productDto)
    {
        if (id != productDto.Id)
        {
            return BadRequest();
        }

        await _productService.UpdateProductAsync(productDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
