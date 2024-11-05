using Application.DTOs.TransactionDto;
using Application.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]

    public class TransactionController(ITransactionService transactionService) : ControllerBase
    {
        private readonly ITransactionService _transactionService = transactionService;
        [HttpGet("GetAllTransaction")]
        public async Task<IActionResult> GetAllTransaction([FromQuery] string? searchByPaymentMethod, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool sortByDate = false)
        {
            var pagedResult = await _transactionService.GetAllTransaction(searchByPaymentMethod, page, pageSize, sortByDate);
            if (pagedResult.IsSuccess == false)
            {
                return BadRequest(pagedResult.AdditionalData);
            }
            return Ok(pagedResult);
        }
        [HttpGet("GetTransactionById/{id}")]
        public async Task<IActionResult> GetTransactionById([FromRoute] Guid id)
        {
            var result = await _transactionService.GetTransactionById(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpPut("UpdateTransaction/{id}")]
        public async Task<IActionResult> UpdateTransaction([FromRoute] Guid id, [FromBody] CreateUpdateTransactionDto request)
        {
            var result = await _transactionService.UpdateTransaction(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpDelete("DeleteTransaction/{id}")]
        public async Task<IActionResult> DeleteTransaction([FromRoute] Guid id)
        {
            var result = await _transactionService.DeleteTransaction(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpGet("GetTransactionByUserId/{userId}")]
        public async Task<IActionResult> GetTransactionByUserId([FromRoute] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool sortByDate = false)
        {
            var result = await _transactionService.GetTransactionByUserId(userId,page, pageSize, sortByDate);
            if (!result.IsSuccess || result.Results.Count() == 0 )
            {
                return NotFound(result.AdditionalData);
            }
            return Ok(result.Results);
        }
        [HttpGet("GetTotalAmountByUserId/{userId}")]
        public async Task<IActionResult> GetTotalAmountByUserId([FromRoute] Guid userId)
        {
            var result = await _transactionService.GetTotalAmountByUserId(userId);
            return Ok(result);
        }
        [HttpGet("paymenttransactions/range")]
        public async Task<IActionResult> GetTransactionsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool sortByDate = false)
        {
            var result = await _transactionService.GetTransactionsByDateRange(startDate, endDate, page, pageSize, sortByDate);
            if (!result.IsSuccess || result.Results.Count() == 0)
            {
                return NotFound(result.AdditionalData);
            }
            return Ok(result.Results);
        }
        [HttpGet]
        [Route("paymenttransactions/statistics")]
        public async Task<ActionResult<Dictionary<PaymentMethod, int>>> GetTransactionStatistics()
        {
            var statistics = await _transactionService.GetTransactionStatistics();

            return Ok(statistics);
        }
    }
}
