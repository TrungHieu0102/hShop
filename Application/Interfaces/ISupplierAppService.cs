using Application.DTOs;


namespace Application.Interfaces
{
    public interface ISupplierAppService
    {
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
        Task<SupplierDto> GetSupplierByIdAsync(Guid id);
        Task AddSupplierAsync(SupplierDto supplierDto);
        Task UpdateSupplierAsync(SupplierDto supplierDto);
        Task DeleteSupplierAsync(Guid id);
    }
}
