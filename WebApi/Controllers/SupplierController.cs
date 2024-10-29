﻿using Application.DTOs;
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
        public async Task<IActionResult> GetAllSuppler([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "", [FromQuery] bool IsDecsending = false)
        {
            var pageResult = await supplierService.GetAllAsync(page, pageSize, search, IsDecsending);
            if (pageResult.IsSuccess)
            {
                return Ok(pageResult.Results);
            }
            return BadRequest();
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSupplierById([FromRoute] Guid id)
        {
            var result = await supplierService.GetByIdAsync(id);
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
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateSupplier([FromRoute] Guid id, [FromForm] CreateUpdateSupplierDto request)
        {
            var result = await supplierService.UpdateSupplier(id, request);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSupplier([FromRoute] Guid id)
        {
            var result = await supplierService.DeleteSupplier(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return NoContent();
        }
    }
}
