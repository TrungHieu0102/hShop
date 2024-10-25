using Application.DTOs.CategoriesDto;
using Application.Extentions;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Model;
using Microsoft.Extensions.Logging;
namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheServices;
        private readonly IPhotoService _photoService;
        private readonly ILogger<ICategoryService> _logger;
        public CategoryService (IUnitOfWorkBase unitOfWork, IMapper mapper, ICacheService cacheService, IPhotoService photoService, ILogger<ICategoryService> logger) => (_unitOfWork, _mapper, _cacheServices, _photoService, _logger) = (unitOfWork, mapper, cacheService, photoService, logger);
        public async Task<Result<CategoryDto>> AddCategoryAsync(CreateUpdateCategoryDto categoryDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var slug = SlugHelper.GenerateSlug(categoryDto.Name);
                if (await _unitOfWork.Categories.IsSlugExits(slug))
                {
                    throw new InvalidOperationException("Slug already exists.");
                }
                var category = _mapper.Map<Category>(categoryDto);
                category.Slug = slug;
                category.Id = Guid.NewGuid();
                if (categoryDto.Images != null)
                {
                    var uploadResult = await _photoService.AddPhotoAsync(categoryDto.Images, "categories");
                    category.PictureUrl = uploadResult.SecureUrl.ToString();

                }
                _unitOfWork.Categories.Add(category);
                var result = await _unitOfWork.CompleteAsync();
                if (result == 0)
                {
                    await transaction.RollbackAsync();
                    return new Result<CategoryDto>
                    {
                        IsSuccess = false,
                        Message = "Failed to add category."
                    };
                }
                await transaction.CommitAsync();
                return new Result<CategoryDto>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CategoryDto>(category)
                };

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new Result<CategoryDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return false;
                }
                if (category.PictureUrl != null)
                {
                    var publicId = PhotoExtensions.ExtractPublicId(category.PictureUrl);
                    await _photoService.DeletePhotoAsync("categories", $"categories/{publicId}");
                }
                _unitOfWork.Categories.Delete(category);
                var result = await _unitOfWork.CompleteAsync();
                if (result == 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<PagedResult<CategoryInListDto>> GetAllAsync(int page, int pageSize, string search, bool IsDecsending = false)
        {

            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    categories = categories.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                categories = IsDecsending ? categories.OrderByDescending(p => p.Name) : categories.OrderBy(p => p.Name);

                var totalRows = categories.Count();

                var pagedCategories = categories.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new PagedResult<CategoryInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = _mapper.Map<IEnumerable<CategoryInListDto>>(pagedCategories),
                    IsSuccess = true
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(message: ex.Message);   
                return new PagedResult<CategoryInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = 0,
                    Results = [],
                    IsSuccess = false
                };
            }
        }
        public async Task<Result<CategoryDto>> GetByIdAsync(Guid id)
        {
            try
            {
                string cachekey = $"Category_{id}";
                var cachedCategory = await _cacheServices.GetCachedDataAsync<Category>(cachekey);
                if (cachedCategory != null)
                {

                    return new Result<CategoryDto>
                    {
                        IsSuccess = true,
                        Data = _mapper.Map<CategoryDto>(cachedCategory)
                    };
                }
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category != null)
                {
                    await _cacheServices.SetCachedDataAsync(cachekey, category);
                }
                return new Result<CategoryDto>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CategoryDto>(category)
                };
            }
            catch (InvalidOperationException ex)
            {
                return new Result<CategoryDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Result<Category>> UpdateCategoryAsync(Guid id, CreateUpdateCategoryDto categoryDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                string slug = SlugHelper.GenerateSlug(categoryDto.Name);
                if (await _unitOfWork.Categories.IsSlugExits(slug))
                {
                    return new Result<Category>
                    {
                        IsSuccess = false,
                        Message = "Slug already exists."
                    };
                }
                if (category.PictureUrl != null)
                {
                    var publicId = PhotoExtensions.ExtractPublicId(category.PictureUrl);
                    await _photoService.DeletePhotoAsync("categories",publicId);
                }
                if (categoryDto.Images != null)
                {
                    var uploadResult = await _photoService.AddPhotoAsync(categoryDto.Images, "categories");
                    category.PictureUrl = uploadResult.SecureUrl.ToString();
                }
                _mapper.Map(categoryDto, category);
                category.Slug = slug;
                _unitOfWork.Categories.Update(category);

                var cacheKey = $"Category_{id}";
                var cache = await _cacheServices.GetCachedDataAsync<Category>(cacheKey);
                if (cache != null)
                {
                    await _cacheServices.RemoveCachedDataAsync(cacheKey);
                }
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return new Result<Category>
                {
                    IsSuccess = true
                };
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                return new Result<Category>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}


