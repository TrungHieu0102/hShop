using Application.DTOs.OrdersDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[ValidateModel]
public class OrderController(IOrderSevice orderService) : ControllerBase
{
    [HttpPost("CreateOrder")]
    [Authorize]
    public async Task<IActionResult> CreateOrder(CreateUpdateOrderDto createUpdateOrderDto)
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest("User not found");
        var userId = Guid.Parse(value); // Lấy UserId từ Claims    
        var result = await orderService.CreateOrderAsync(userId, createUpdateOrderDto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }
}