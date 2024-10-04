using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class SupplierAppService : ISupplierAppService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkBase _unitOfWork;
        public SupplierAppService(ISupplierRepository supplierRepository, IMapper mapper, IUnitOfWorkBase unitOfWork)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task AddSupplierAsync(SupplierDto supplierDto)
        {
            await _supplierRepository.AddAsync(_mapper.Map<Supplier>(supplierDto));
        }

        public async Task DeleteSupplierAsync(Guid id)
        {
            await _supplierRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<SupplierDto> GetSupplierByIdAsync(Guid id)
        {
           var supplier = await _supplierRepository.GetByIdAsync(id);
            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task UpdateSupplierAsync(SupplierDto supplierDto)
        {
            var supplier = _mapper.Map<Supplier>(supplierDto);
            await _supplierRepository.UpdateAsync(supplier);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
