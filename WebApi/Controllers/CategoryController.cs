using Application.DTOs.CategoriesDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[ValidateModel]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService= categoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "", [FromQuery] bool IsDecsending = false)
    {
        var pagedResult = await _categoryService.GetAllAsync(page, pageSize, search, IsDecsending);
        return Ok(pagedResult);
    }
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CreateUpdateCategoryDto categoryDto)
    {
       
        try
        {
            var resutl = await _categoryService.AddCategoryAsync(categoryDto);
            if (resutl == 0)
            {
                return StatusCode(500, "Failed to create category");
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
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CreateUpdateCategoryDto categoryDto)
    {
      
        var result = await _categoryService.UpdateCategoryAsync(id, categoryDto);
        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }
        return NoContent();
    }
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result)
        {
            return NotFound("Category not found");
        }
        return NoContent();
    }
}