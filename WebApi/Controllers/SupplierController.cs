//using Application.DTOs;
//using Application.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace WebApi.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class SupplierController : ControllerBase
//    {
//        private readonly ISupplierAppService _supplierService;
//        public SupplierController(ISupplierAppService supplierAppService)
//        {
//            _supplierService = supplierAppService;
//        }
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAllSuppliers()
//        {
//            var suppliers = await _supplierService.GetAllSuppliersAsync();
//            return Ok(suppliers);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<SupplierDto>> GetSupplieryId(Guid id)
//        {
//            var supplier = await _supplierService.GetSupplierByIdAsync(id);
//            if (supplier == null)
//            {
//                return NotFound();
//            }
//            return Ok(supplier);
//        }

//        [HttpPost]
//        public async Task<ActionResult> AddSupplier(SupplierDto supplierDto)
//        {
//            await _supplierService.AddSupplierAsync(supplierDto);
//            return CreatedAtAction(nameof(GetSupplieryId), new { id = supplierDto.Id }, supplierDto);
//        }

//        [HttpPut("{id}")]
//        public async Task<ActionResult> UpdateSupplier(Guid id, SupplierDto supplierDto)
//        {
//            if (id != supplierDto.Id)
//            {
//                return BadRequest();
//            }

//            await _supplierService.UpdateSupplierAsync(supplierDto);
//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<ActionResult> DeleteSupplier(Guid id)
//        {
//            await _supplierService.DeleteSupplierAsync(id);
//            return NoContent();
//        }
//    }
//}
