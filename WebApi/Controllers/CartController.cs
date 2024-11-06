using Application.Interfaces;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpPost("Add")]
    [Authorize]
    public async Task<IActionResult> AddToCart([FromQuery] Guid productId, [FromQuery] int quantity)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return BadRequest("User not found");
        var userId = Guid.Parse(userIdClaim);
        var result = await cartService.AddToCartAsync(userId, productId, quantity);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpDelete("Remove")]
    [Authorize]
    public async Task<IActionResult> RemoveFromCart([FromQuery] Guid productId)
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest();
        var userId = Guid.Parse(value);
        var result = await cartService.RemoveFromCartAsync(userId, productId);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpDelete("Clear")]
    [Authorize]
    public async Task<IActionResult> ClearCart()
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest();
        var userId = Guid.Parse(value);
        var result = await cartService.ClearCartAsync(userId);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpGet("Get")]
    [Authorize]
    public async Task<IActionResult> GetCartDetails()
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest("Please login");
        var userId = Guid.Parse(value);
        var result = await cartService.GetCartDetailsAsync(userId);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpPut("Update/{productId}")]
    [Authorize]
    public async Task<IActionResult> UpdateCartQuantity([FromRoute] Guid productId, [FromQuery] int quantity)
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest("Please login");
        var userId = Guid.Parse(value);
        var result = await cartService.UpdateCartQuantityAsync(userId, productId, quantity);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return NoContent();
    }
}