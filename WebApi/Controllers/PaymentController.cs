using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaypalService _paypalService;
        private readonly IVnPayService _vnPayService;

        public PaymentController(IPaypalService paypalService, IVnPayService vnPayService)
        {
            _paypalService = paypalService;
            _vnPayService = vnPayService;
        }

        [HttpPost("paypal/create-payment/{orderId}")]
        public async Task<IActionResult> CreatePayment([FromRoute] Guid orderId)
        {
            var result = await _paypalService.CreatePaymentAsync(orderId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { redirectUrl = result.Data });
        }

        [HttpGet("paypal/execute-payment")]
        public async Task<IActionResult> ExecutePayment([FromQuery] string paymentId, [FromQuery] string payerId,
            [FromQuery] Guid orderId)
        {
            var result = await _paypalService.ExecutePaymentAsync(paymentId, payerId, orderId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message });
        }

        [HttpGet("paypal/cancel")]
        public IActionResult Cancel()
        {
            return BadRequest("Payment was canceled.");
        }

        [HttpPost("vnpay/create-payment/{orderId:guid}")]
        public async Task<IActionResult> CreatePaymentUrl([FromRoute] Guid orderId)
        {
            try
            {
                var paymentUrl = await _vnPayService.CreatePaymentUrl(HttpContext, orderId);
                return Ok(new { PaymentUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpGet("vnpay/callback")]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = await _vnPayService.PaymentExecute(Request.Query);
            if (response.Success)
            {
                return Ok(new { Message = "Payment successful", Data = response });
            }
            else
            {
                return BadRequest(new { Message = "Payment failed." });
            }
        }
    }
}