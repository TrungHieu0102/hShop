using Application.Interfaces;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddToCart(Guid productId, int quantity)
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest("User not found");
        var userId = Guid.Parse(value); // Lấy UserId từ Claims
        var result = await cartService.AddToCartAsync(userId, productId, quantity);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpDelete("remove")]
    [Authorize]
    public async Task<IActionResult> RemoveFromCart(Guid productId)
    {
        var value = User.FindFirst("id")?.Value;
        if (value == null) return BadRequest();
        var userId = Guid.Parse(value);
        var result = await cartService.RemoveFromCartAsync(userId, productId);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpDelete("clear")]
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

    [HttpGet]
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

    [HttpPut("update-cart-quantity ")]
    [Authorize]
    public async Task<IActionResult> UpdateCartQuantity(Guid productId, int quantity)
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