using Application.DTOs.CategoriesDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ValidateModel]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string search = "", [FromQuery] bool IsDecsending = false)
    {
        var pagedResult = await categoryService.GetAllAsync(page, pageSize, search, IsDecsending);
        if (pagedResult.IsSuccess == false)
        {
            return BadRequest(pagedResult.AdditionalData);
        }

        return Ok(pagedResult);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var result = await categoryService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromForm] CreateUpdateCategoryDto categoryDto)
    {
        var result = await categoryService.AddCategoryAsync(categoryDto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Data.Id }, result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromForm] CreateUpdateCategoryDto categoryDto)
    {
        var result = await categoryService.UpdateCategoryAsync(id, categoryDto);
        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await categoryService.DeleteCategoryAsync(id);
        if (!result)
        {
            return NotFound("Category not found");
        }

        return NoContent();
    }
}