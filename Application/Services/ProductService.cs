using Application.DTOs.CategoriesDto;
using Application.DTOs.ProductsDto;
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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly ICacheService _cacheServices;
        private readonly ILogger<IProductService> _logger;

        public ProductService(IUnitOfWorkBase unitOfWork, IMapper mapper, IPhotoService photoService, ICacheService cacheServices, ILogger<IProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
            _cacheServices = cacheServices;
            _logger = logger;
        }
        public async Task<Result<Product>> AddProductAsync(CreateUpdateProductDto productDto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var slug = SlugHelper.GenerateSlug(productDto.Name);
                if (await _unitOfWork.Products.IsSlugExits(slug))
                {
                    return new Result<Product>()
                    {
                        IsSuccess = false,
                        Message = $"This {slug}  slug already exists!",
                    };
                }

                var product = _mapper.Map<Product>(productDto);
                product.Slug = slug;
                product.Id = Guid.NewGuid();
                product.Images = new List<ProductImage>();
                if (productDto.Images.Count > 0)
                {
                    foreach (var image in productDto.Images)
                    {
                        var uploadResult = await _photoService.AddPhotoAsync(image, "product");
                        if (uploadResult.Error == null)
                        {
                            product.Images.Add(new ProductImage
                            {
                                Id = Guid.NewGuid(),
                                ProductId = product.Id,
                                ImageUrl = uploadResult.SecureUrl.ToString(),
                                IsPrimary = product.Images.Count == 0,
                                ImageType = "Product"
                            });
                        }
                    }
                }

                _unitOfWork.Products.Add(product);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return new Result<Product>()
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
                return new Result<Product>()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Result<Product>> UpdateProductWithImagesAsync(Guid id, CreateUpdateProductDto productDto)
        {
            await using var trasaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var productUpdateResult = await UpdateProductAsync(id, productDto);
                if (!productUpdateResult.IsSuccess)
                {
                    return productUpdateResult;
                }
                if (productDto.Images.Count != 0)
                {
                    var imageUpdateResult = await _photoService.UpdateProductImagesAsync(id, productDto.Images);
                    if (!imageUpdateResult.IsSuccess)
                    {
                        return new Result<Product> { IsSuccess = false, Message = "Failed to update product images." };
                    }
                }
                await trasaction.CommitAsync();
                return productUpdateResult;
            }
            catch (Exception ex)
            {
                await trasaction.RollbackAsync();
                return new Result<Product> { IsSuccess = false, Message = ex.Message };
            }
        }
        public async Task<Result<Product>> UpdateProductAsync(Guid id, CreateUpdateProductDto productDto)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return new Result<Product> { IsSuccess = false, Message = "Product not found." };
                }

                var slug = SlugHelper.GenerateSlug(productDto.Name);
                if (await _unitOfWork.Products.IsSlugExits(slug))
                {
                    return new Result<Product> { IsSuccess = false, Message = "Slug already exists." };
                }
                _mapper.Map(productDto, product);
                product.Slug = slug;
                var cachKey = $"Product_{id}";
                var cache = await _cacheServices.GetCachedDataAsync<Product>(cachKey);
                if (cache != null)
                {
                    await _cacheServices.RemoveCachedDataAsync(cachKey);
                }
                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();

                return new Result<Product> { IsSuccess = true, Data = product };
            }
            catch (Exception ex)
            {
                return new Result<Product> { IsSuccess = false, Message = ex.Message };
            }
        }
        public async Task<bool> DeleteProductAsync(Guid id)
        {
           await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await _unitOfWork.Products.GetProductWithImagesByIdAsync(id);
                if (product == null)
                {
                    return false;
                }
                if (product.Images != null && product.Images.Any())
                {
                    foreach (var image in product.Images)
                    {
                        var publicId = PhotoExtensions.ExtractPublicId(image.ImageUrl);
                        await _photoService.DeletePhotoAsync("product", publicId);
                        _unitOfWork.Images.Delete(image);
                    }
                }
                var cacheKey = $"Product_{id}";
                var cache = await _cacheServices.GetCachedDataAsync<Product>(cacheKey);
                if (cache != null)
                {
                    await _cacheServices.RemoveCachedDataAsync(cacheKey);
                }
                _unitOfWork.Products.Delete(product);

                var result = await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return result > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<PagedResult<ProductInListDto>> GetAllProductsAsync(int page, int pageSize, string search, bool IsDecsending = false)
        {
            try
            {
                var productsQuery = await _unitOfWork.Products.GetProductsWithImagesAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    productsQuery = productsQuery.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                productsQuery = IsDecsending ? productsQuery.OrderByDescending(p => p.Name) : productsQuery.OrderBy(p => p.Name);

                var totalRows =  productsQuery.Count();

                var pagedProducts = productsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductInListDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Slug = p.Slug,
                        Description = p.Description,
                        Price = p.Price,
                        Unit = p.Unit,
                        Discount = p.Discount,
                        DateCreated = p.DateCreated,
                        ViewCount = p.ViewCount,
                        Images = p.Images.Select(pi => new ProductImageDto
                        {
                            ImageUrl = pi.ImageUrl
                        }).ToList()
                    })
                    .ToList();

                return new PagedResult<ProductInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = pagedProducts,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting products.");
                return new PagedResult<ProductInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = 0,
                    Results = [],
                    IsSuccess = false
                };
            }
        }

        public async Task<PagedResult<ProductDto>> GetProductByCategoryAsync(Guid categoryId, int page, int pageSize, bool IsDecsending)
        {
            try
            {
                var products = await _unitOfWork.Products.GetByCategoryIdAsync(categoryId);
                products = IsDecsending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name);
                var totalRows = products.Count();
                var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PagedResult<ProductDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts),
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new PagedResult<ProductDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = 0,
                    Results = [],
                    IsSuccess = false
                };
            }
        }
        public async Task<Result<ProductDto>> GetProductByIdAsync(Guid id)
        {
            try
            {
                var cacheKey = $"Product_{id}";
                var cacheResult = await _cacheServices.GetCachedDataAsync<ProductDto>(cacheKey);
                if (cacheResult != null)
                {
                    return new Result<ProductDto>
                    {
                        IsSuccess = true,
                        Data = cacheResult
                    };
                }
                var product = await _unitOfWork.Products.GetProductWithImagesByIdAsync(id);
                var productDto = _mapper.Map<ProductDto>(product);
                await _cacheServices.SetCachedDataAsync(cacheKey, productDto);
                return new Result<ProductDto>
                {
                    IsSuccess = true,
                    Data = productDto
                };
            }
            catch (InvalidOperationException)
            {
                return new Result<ProductDto>
                {
                    IsSuccess = false,
                    Message = "Product not found."
                };
            }
        }
        public async Task<PagedResult<ProductInListDto>> SearchProductByNameAsync(string name, int page, int pageSize, bool IsDecsending)
        {
            try
            {
                var products = await _unitOfWork.Products.SearchByNameAsync(name);
                products = IsDecsending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name);
                var totalRows = products.Count();
                var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PagedResult<ProductInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = _mapper.Map<IEnumerable<ProductInListDto>>(pagedProducts),
                    IsSuccess = true
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new PagedResult<ProductInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = 0,
                    Results = new List<ProductInListDto>(),
                    IsSuccess = false
                };
            }
        }
    }
}
