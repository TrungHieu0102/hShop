using Application.DTOs.OrdersDto;
using Application.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ValidateModel]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrder([FromBody] CreateUpdateOrderDto createUpdateOrderDto)
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest("User not found");
        var userId = Guid.Parse(value);
        var result = await orderService.CreateOrderAsync(userId, createUpdateOrderDto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid id)
    {
        var result = await orderService.GetOrderAsync(id);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpGet("user/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetOrdersByUserId([FromRoute] Guid userId, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10, [FromQuery] bool IsDecsending = false)
    {
        var result = await orderService.GetOrderByUserIdAsync(userId, page, pageSize, IsDecsending);
        if (!result.IsSuccess)
        {
            return BadRequest();
        }

        return Ok(result.Results);
    }

    [HttpGet("user/orders")]
    [Authorize]
    public async Task<IActionResult> GetOrdersByUserClaim([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] bool isDecsending = false)
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest("Please login");
        var userId = Guid.Parse(value);
        var result = await orderService.GetOrderByUserClaimAsync(userId, page, pageSize, isDecsending);
        if (!result.IsSuccess)
        {
            return BadRequest();
        }

        return Ok(result.Results);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllOrder([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] bool isDecsending = false)
    {
        var result = await orderService.GetAllOrderAsync(page, pageSize, isDecsending);
        if (!result.IsSuccess)
        {
            return BadRequest();
        }

        return Ok(result.Results);
    }

    [HttpPut("payment-status")]
    public async Task<IActionResult> UpdatePaymentStatus([FromBody] UpdateStatusDto<PaymentStatus> dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await orderService.UpdatePaymentStatusAsync(dto.OrderId, dto.Status);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpPut("order-status")]
    public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateStatusDto<OrderStatus> dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await orderService.UpdateOrderStatusAsync(dto.OrderId, dto.Status);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpGet("status")]
    [Authorize]
    public async Task<IActionResult> GetOrdersByStatus([FromQuery] OrderStatus status, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10, [FromQuery] bool isDecsending = false)
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest("Please login");
        var userId = Guid.Parse(value);
        var result = await orderService.GetOrdersByStatusAsync(userId, status, page, pageSize, isDecsending);
        if (!result.IsSuccess)
        {
            return BadRequest("Order not found");
        }

        return Ok(result.Results);
    }

    [HttpPut("{orderId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateOrder([FromRoute] Guid orderId,
        [FromBody] CreateUpdateOrderDto createUpdateOrderDto)
    {
        var result = await orderService.UpdateOrderAsync(orderId, createUpdateOrderDto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpDelete("cancelled")]
    [Authorize]
    public async Task<IActionResult> DeleteAllOrderCanceled()
    {
        var result = await orderService.DeleteAllOrderCanceled();
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok("Order deleted");
    }
}