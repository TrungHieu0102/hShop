using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult> AddProduct([FromBody] CreateUpdateProductDto productDto)
    {
        if (!ModelState.IsValid)      
            return BadRequest(ModelState);
        try
        {
            var resutl = await _productService.AddProductAsync(productDto);
            if(resutl== 0)
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
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] CreateUpdateProductDto productDto)
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
        if(!await _productService.DeleteProductAsync(id))
        {
            return BadRequest();
        }
        return NoContent();

    }
}
