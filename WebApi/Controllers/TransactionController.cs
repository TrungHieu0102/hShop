using Application.DTOs.TransactionDto;
using Application.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]
    public class TransactionController(ITransactionService transactionService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllTransaction([FromQuery] string? searchByPaymentMethod,
                                                            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
                                                            [FromQuery] bool sortByDate = false)
        {
            var pagedResult = await transactionService.GetAllTransaction(searchByPaymentMethod, page, pageSize, sortByDate);
            if (pagedResult.IsSuccess == false)
            {
                return BadRequest(pagedResult.AdditionalData);
            }
            return Ok(pagedResult);
        }
        [HttpGet("{transactionId:guid}")]
        public async Task<IActionResult> GetTransactionById([FromRoute] Guid transactionId)
        {
            var result = await transactionService.GetTransactionById(transactionId);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpPut("{transactionId:guid}")]
        public async Task<IActionResult> UpdateTransaction([FromRoute] Guid transactionId, [FromBody] CreateUpdateTransactionDto request)
        {
            var result = await transactionService.UpdateTransaction(transactionId, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpDelete("{transactionId:guid}")]
        public async Task<IActionResult> DeleteTransaction([FromRoute] Guid transactionId)
        {
            var result = await transactionService.DeleteTransaction(transactionId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetTransactionByUserId([FromRoute] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool sortByDate = false)
        {
            var result = await transactionService.GetTransactionByUserId(userId, page, pageSize, sortByDate);
            if (!result.IsSuccess || result.Results.Count() == 0)
            {
                return NotFound(result.AdditionalData);
            }
            return Ok(result.Results);
        }
        [HttpGet("total-amount/{userId:guid}")]
        public async Task<IActionResult> GetTotalAmountByUserId([FromRoute] Guid userId)
        {
            var result = await transactionService.GetTotalAmountByUserId(userId);
            return Ok(result);
        }
        [HttpGet("payment/date/range")]
        public async Task<IActionResult> GetTransactionsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool sortByDate = false)
        {
            var result = await transactionService.GetTransactionsByDateRange(startDate, endDate, page, pageSize, sortByDate);
            if (!result.IsSuccess || result.Results.Count() == 0)
            {
                return NotFound(result.AdditionalData);
            }
            return Ok(result.Results);
        }
        [HttpGet]
        [Route("payment/statistics")]
        public async Task<ActionResult<Dictionary<PaymentMethod, int>>> GetTransactionStatistics()
        {
            var statistics = await transactionService.GetTransactionStatistics();

            return Ok(statistics);
        }
    }
}
