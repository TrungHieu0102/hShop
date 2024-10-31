using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-payment/{orderId}")]
        public async Task<IActionResult> CreatePayment(Guid orderId)
        {
            var result = await _paymentService.CreatePaymentAsync(orderId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { redirectUrl = result.Data });
        }

        [HttpGet("execute-payment")]
        public async Task<IActionResult> ExecutePayment(string paymentId, string payerId, Guid orderId)
        {
            var result = await _paymentService.ExecutePaymentAsync(paymentId, payerId, orderId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Message = result.Message });
        }

        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            return BadRequest("Payment was canceled.");
        }
    }
}
