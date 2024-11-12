using Application.DTOs.ReviewDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpPost]
        public async Task<IActionResult> AddReview([FromForm] CreateUpdateReviewDto reviewRequest)
        {
            var result = await _reviewService.CreateReview(reviewRequest);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById([FromRoute] Guid id)
        {
            var result = await _reviewService.GetReviewById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewByProductId([FromRoute] Guid productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool isDescending = false)
        {
            var result = await _reviewService.GetAllReviewByProductId(productId, page, pageSize, isDescending);
            if (!result.IsSuccess)
            {
                return BadRequest(result.AdditionalData);
            }
            return Ok(result.Results);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview([FromRoute] Guid id, [FromForm] CreateUpdateReviewDto request)
        {
            var result = await _reviewService.UpdateReviewWithImages(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewById([FromRoute] Guid id)
        {
            var result = await _reviewService.DeleteReviewById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
        [Authorize]
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteReviewByUser([FromRoute] Guid id)
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (userIdClaim == null) return BadRequest("User not found");
            var userId = Guid.Parse(userIdClaim);
            var result = await _reviewService.DeleteReviewByUser(id, userId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
    }
}
