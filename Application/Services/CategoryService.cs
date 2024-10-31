using Application.DTOs.CategoriesDto;
using Application.Extensions;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Model;
using Microsoft.Extensions.Logging;
namespace Application.Services
{
    public class CategoryService(
        IUnitOfWorkBase unitOfWork,
        IMapper mapper,
        ICacheService cacheService,
        IPhotoService photoService,
        ILogger<ICategoryService> logger)
        : ICategoryService
    {
        public async Task<Result<CategoryDto>> AddCategoryAsync(CreateUpdateCategoryDto categoryDto)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var slug = SlugHelper.GenerateSlug(categoryDto.Name);
                if (await unitOfWork.Categories.IsSlugExits(slug))
                {
                    throw new InvalidOperationException("Slug already exists.");
                }
                var category = mapper.Map<Category>(categoryDto);
                category.Slug = slug;
                category.Id = Guid.NewGuid();
                if (categoryDto.Images != null)
                {
                    var uploadResult = await photoService.AddPhotoAsync(categoryDto.Images, "categories");
                    category.PictureUrl = uploadResult.SecureUrl.ToString();

                }
                unitOfWork.Categories.Add(category);
                var result = await unitOfWork.CompleteAsync();
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
                    Data = mapper.Map<CategoryDto>(category)
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
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var category = await unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return false;
                }
                if (category.PictureUrl != null)
                {
                    var publicId = PhotoExtensions.ExtractPublicId(category.PictureUrl);
                    await photoService.DeletePhotoAsync("categories", $"categories/{publicId}");
                }
                unitOfWork.Categories.Delete(category);
                var result = await unitOfWork.CompleteAsync();
                if (result == 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                var cacheKey = $"Category_{id}";
                await cacheService.RemoveCachedDataAsync(cacheKey);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<PagedResult<CategoryInListDto>> GetAllAsync(int page, int pageSize, string search, bool isDecsending = false)
        {

            try
            {
                var categories = await unitOfWork.Categories.GetAllAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    categories = categories.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                categories = isDecsending ? categories.OrderByDescending(p => p.Name) : categories.OrderBy(p => p.Name);

                var totalRows = categories.Count();

                var pagedCategories = categories.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new PagedResult<CategoryInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = mapper.Map<IEnumerable<CategoryInListDto>>(pagedCategories),
                    IsSuccess = true
                };
            }
            catch(Exception ex)
            {
                logger.LogError(message: ex.Message);   
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
                var cacheKey = $"Category_{id}";
                var cachedCategory = await cacheService.GetCachedDataAsync<Category>(cacheKey);
                if (cachedCategory != null)
                {

                    return new Result<CategoryDto>
                    {
                        IsSuccess = true,
                        Data = mapper.Map<CategoryDto>(cachedCategory)
                    };
                }
                var category = await unitOfWork.Categories.GetByIdAsync(id);
                if (category != null)
                {
                    await cacheService.SetCachedDataAsync(cacheKey, category);
                }
                return new Result<CategoryDto>
                {
                    IsSuccess = true,
                    Data = mapper.Map<CategoryDto>(category)
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
            await using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                var category = await unitOfWork.Categories.GetByIdAsync(id);
                var slug = SlugHelper.GenerateSlug(categoryDto.Name);
                if (await unitOfWork.Categories.IsSlugExits(slug))
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
                    await photoService.DeletePhotoAsync("categories",publicId);
                }
                if (categoryDto.Images != null)
                {
                    var uploadResult = await photoService.AddPhotoAsync(categoryDto.Images, "categories");
                    category.PictureUrl = uploadResult.SecureUrl.ToString();
                }
                mapper.Map(categoryDto, category);
                category.Slug = slug;
                unitOfWork.Categories.Update(category);

                var cacheKey = $"Category_{id}";
                var cache = await cacheService.GetCachedDataAsync<Category>(cacheKey);
                if (cache != null)
                {
                    await cacheService.RemoveCachedDataAsync(cacheKey);
                }
                await unitOfWork.CompleteAsync();
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


