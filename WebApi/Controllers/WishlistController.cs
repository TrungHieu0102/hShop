using Application.DTOs.WishlistDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        [HttpPost]
        public async Task<IActionResult> CreatateWishlist([FromBody] CreateUpdateWishlistDto request)
        {
            var result = await _wishlistService.AddWishlist(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpDelete("{userId:guid}/{productId:guid}")]
        public async Task<IActionResult> DeleteWishlist([FromRoute]Guid userId,[FromRoute] Guid productId)
        {
            var result = await _wishlistService.DeleteWishlist(userId, productId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetWishlistByUserId([FromRoute] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _wishlistService.GetWishlistByUserid(userId, page, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest();
            }
            return Ok(result.Results);
        }
    }
}
