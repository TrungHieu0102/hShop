using Application.Interfaces;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CartController:ControllerBase
{
    private readonly ICartService   _cartService;
    public CartController(ICartService cartService)
    {
        _cartService = cartService; 
    }

    // Thêm sản phẩm vào giỏ hàng
    [HttpPost("add")]
    [Authorize] // Chỉ cho phép người dùng đã đăng nhập
    public async Task<IActionResult> AddToCart(Guid productId, int quantity)
    {
        var userId = Guid.Parse(User.FindFirst("id")?.Value); // Lấy UserId từ Claims
        var result = await _cartService.AddToCartAsync(userId, productId, quantity);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    // Xóa sản phẩm khỏi giỏ hàng
    [HttpDelete("remove")]
    [Authorize]
    public async Task<IActionResult> RemoveFromCart(Guid productId)
    {
        var userId = Guid.Parse(User.FindFirst("id")?.Value);
        var result = await _cartService.RemoveFromCartAsync(userId, productId);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    // Xóa toàn bộ giỏ hàng
    [HttpDelete("clear")]
    [Authorize]
    public async Task<IActionResult> ClearCart()
    {
        var userId = Guid.Parse(User.FindFirst("id")?.Value);
        var result = await _cartService.ClearCartAsync(userId);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }
}
