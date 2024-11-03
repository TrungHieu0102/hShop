using Application.DTOs.OrdersDto;
using Application.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[ValidateModel]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost("CreateOrder")]
    [Authorize]
    public async Task<IActionResult> CreateOrder(CreateUpdateOrderDto createUpdateOrderDto)
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

    [HttpGet("GetOrderById/{id}")]
    [Authorize]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var result = await orderService.GetOrderAsync(id);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpGet("GetOrdersByUserId/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetOrdersByUserId(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10,  [FromQuery] bool IsDecsending = false)
    {
        var result = await orderService.GetOrderByUserIdAsync(userId, page, pageSize, IsDecsending);
        if (!result.IsSuccess)
        {
            return BadRequest();
        }

        return Ok(result.Results);
    }

    [HttpGet("GetOrdersByUserClaim")]
    [Authorize]
    public async Task<IActionResult> GetOrdersByUserClaim([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool isDecsending = false)
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

    [HttpGet("GetAllOrder")]
    [Authorize]
    public async Task<IActionResult> GetAllOrder([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool isDecsending = false)
    {
        var result = await orderService.GetAllOrderAsync(page, pageSize, isDecsending);
        if (!result.IsSuccess)
        {
            return BadRequest();
        }
        return Ok(result.Results);
    }
   
    [HttpPut("update-payment-status")]
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
    [HttpPut("update-order-status")]
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

    [HttpGet("GetOrdersByStatus")]
    [Authorize]
    public async Task<IActionResult> GetOrdersByStatus(OrderStatus status, [FromQuery] int page = 1,
                                            [FromQuery] int pageSize = 10, [FromQuery] bool isDecsending = false)
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest("Please login");
        var userId = Guid.Parse(value);
        var result = await orderService.GetOrdersByStatusAsync(userId,status, page, pageSize, isDecsending);
        if (!result.IsSuccess )
        {
            return BadRequest("Order not found");
        }
        return Ok(result.Results);
    }
    [HttpPut("UpdateOrder{orderId}")]
    [Authorize]
    public async Task<IActionResult> UpdateOrder(Guid orderId, CreateUpdateOrderDto createUpdateOrderDto)
    {
        var result = await orderService.UpdateOrderAsync(orderId, createUpdateOrderDto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result.Data);
    }
    [HttpDelete]
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