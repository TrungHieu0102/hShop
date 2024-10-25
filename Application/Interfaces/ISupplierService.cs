using Application.DTOs.SuppliersDto;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISupplierService
    {
        Task<Result<SupplierDto>> AddSupplier(CreateUpdateSupplierDto request);
        Task<Result<SupplierDto>> UpdateSupplier(Guid id, CreateUpdateSupplierDto request);
        Task<Result<SupplierDto>> DeleteSupplier(Guid id);
        Task<Result<SupplierDto>> GetByIdAsync(Guid id);
        Task<PagedResult<SupplierInListDto>> GetAllAsync(int page, int pageSize, string search, bool IsDecsending = false);
    }
}
