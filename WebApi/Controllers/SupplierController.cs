using Application.DTOs.SuppliersDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController(ISupplierService supplierService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllSuppler([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "", [FromQuery] bool isDecsending = false)
        {
            var pageResult = await supplierService.GetAllAsync(page, pageSize, search, isDecsending);
            if (pageResult.IsSuccess)
            {
                return Ok(pageResult.Results);
            }
            return BadRequest();
        }
        [HttpGet("{supplierId:guid}")]
        public async Task<IActionResult> GetSupplierById([FromRoute] Guid supplierId)
        {
            var result = await supplierService.GetByIdAsync(supplierId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromForm] CreateUpdateSupplierDto request)
        {
            var result = await supplierService.AddSupplier(request);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest();
        }
        [HttpPut("{supplierId:guid}")]
        public async Task<IActionResult> UpdateSupplier([FromRoute] Guid supplierId, [FromForm] CreateUpdateSupplierDto request)
        {
            var result = await supplierService.UpdateSupplier(supplierId, request);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }
        [HttpDelete("{supplierId:guid}")]
        public async Task<IActionResult> DeleteSupplier([FromRoute] Guid supplierId)
        {
            var result = await supplierService.DeleteSupplier(supplierId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return NoContent();
        }
    }
}
