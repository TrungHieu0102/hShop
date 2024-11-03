using Application.DTOs.SuppliersDto;
using Application.Extensions;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class SupplierService(
        IUnitOfWorkBase unitOfWork,
        IPhotoService photoService,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<ISupplierService> logger)
        : ISupplierService
    {
        private readonly IUnitOfWorkBase _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly IPhotoService _photoService = photoService ?? throw new ArgumentNullException(nameof(photoService));
        private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<ISupplierService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        public async Task<Result<SupplierDto>> AddSupplier(CreateUpdateSupplierDto request)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var supplier = _mapper.Map<Supplier>(request);
                supplier.Id = Guid.NewGuid();
                if (request.Logo != null)
                {
                    var uploadResult = await _photoService.AddPhotoAsync(request.Logo, "suppliers");
                    supplier.Logo = uploadResult.SecureUrl.ToString();
                }
                _unitOfWork.Suppliers.Add(supplier);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return new Result<SupplierDto>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<SupplierDto>(supplier)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new Result<SupplierDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Result<SupplierDto>> DeleteSupplier(Guid id)
        {
           await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cacheKey = $"Supplier:{id}";
                var cacheItem =await cacheService.GetCachedDataAsync<SupplierDto>(cacheKey);
                var supplier = mapper.Map<Supplier>(cacheItem) 
                               ?? await _unitOfWork.Suppliers.GetByIdAsync(id)
                               ?? throw new Exception($"Supplier with id: {id} could not be found");
                if (supplier.Logo != null)
                {
                    var publicId = PhotoExtensions.ExtractPublicId(supplier.Logo);
                    await _photoService.DeletePhotoAsync("suppliers", $"suppliers/{publicId}");
                }
                _unitOfWork.Suppliers.Delete(supplier);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                await _cacheService.RemoveCachedDataAsync(cacheKey);
                return new Result<SupplierDto>
                {
                    IsSuccess = true,
                    Message = "Supplier deleted successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(message: ex.Message);
                return new Result<SupplierDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<PagedResult<SupplierInListDto>> GetAllAsync(int page, int pageSize, string search , bool isDecsending = false)
        {
            try
            {
                var suppliers = await _unitOfWork.Suppliers.GetAllAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    suppliers = suppliers.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                suppliers = isDecsending ? suppliers.OrderByDescending(p => p.Name) : suppliers.OrderBy(p => p.Name);

                var totalRows = suppliers.Count();

                var pagedSuppliers = suppliers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new PagedResult<SupplierInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = _mapper.Map<IEnumerable<SupplierInListDto>>(pagedSuppliers),
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message);
                return new PagedResult<SupplierInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = 0,
                    Results = [],
                    IsSuccess = false
                };
            }
        }

        public async Task<Result<SupplierDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var cacheKey = $"Supplier-{id}";
                var cachedItem = await _cacheService.GetCachedDataAsync<SupplierDto>(cacheKey);
                var supplier = mapper.Map<Supplier>(cachedItem)
                               ?? await unitOfWork.Suppliers.GetByIdAsync(id)
                               ?? throw new Exception("Supplier not found");
                var response = mapper.Map<SupplierDto>(supplier);
                await _cacheService.SetCachedDataAsync(cacheKey, supplier);
                return new Result<SupplierDto>
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new Result<SupplierDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Result<SupplierDto>> UpdateSupplier(Guid id, CreateUpdateSupplierDto request)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cacheKey = $"Supplier-{id}";
                var cacheItem =await cacheService.GetCachedDataAsync<SupplierDto>(cacheKey);
                var supplier = mapper.Map<Supplier>(cacheItem) 
                               ?? await _unitOfWork.Suppliers.GetByIdAsync(id) 
                               ?? throw new Exception("Supplier not found.");
                if (supplier.Logo != null)
                {
                    var publicId = PhotoExtensions.ExtractPublicId(supplier.Logo);
                    await _photoService.DeletePhotoAsync("suppliers", publicId);
                }
                _mapper.Map(request, supplier);

                if (request.Logo != null)
                {
                    var uploadResult = await _photoService.AddPhotoAsync(request.Logo, "suppliers");
                    supplier.Logo = uploadResult.SecureUrl.ToString();
                }

                if (await _cacheService.GetCachedDataAsync<SupplierDto>(cacheKey) != null)
                {
                    await _cacheService.RemoveCachedDataAsync(cacheKey);
                }
                unitOfWork.Suppliers.Update(supplier);    
                var result = await _unitOfWork.CompleteAsync();
                if (result == 0)
                {
                    await transaction.RollbackAsync();
                    return new Result<SupplierDto>
                    {
                        IsSuccess = false,
                        Message = "Failed to update supplier."
                    };
                }
                _unitOfWork.Suppliers.Update(supplier);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return new Result<SupplierDto>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<SupplierDto>(supplier)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new Result<SupplierDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
