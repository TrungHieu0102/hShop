using Application.DTOs.CategoriesDto;
using Application.DTOs.ProductsDto;
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
    public class ProductService(
        IUnitOfWorkBase unitOfWork,
        IMapper mapper,
        IPhotoService photoService,
        ICacheService cacheServices,
        ILogger<IProductService> logger)
        : IProductService
    {
        public async Task<Result<Product>> AddProductAsync(CreateUpdateProductDto productDto)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var slug = SlugHelper.GenerateSlug(productDto.Name);
                if (await unitOfWork.Products.IsSlugExits(slug))
                {
                    return new Result<Product>()
                    {
                        IsSuccess = false,
                        Message = $"This {slug}  slug already exists!",
                    };
                }

                var product = mapper.Map<Product>(productDto);
                product.Slug = slug;
                product.Id = Guid.NewGuid();
                product.Images = new List<ProductImage>();
                if (productDto.Images.Count > 0)
                {
                    foreach (var image in productDto.Images)
                    {
                        var uploadResult = await photoService.AddPhotoAsync(image, "product");
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

                unitOfWork.Products.Add(product);
                await unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return new Result<Product>()
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
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
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var productUpdateResult = await UpdateProductAsync(id, productDto);
                if (!productUpdateResult.IsSuccess)
                {
                    return productUpdateResult;
                }

                if (productDto.Images.Count != 0)
                {
                    var imageUpdateResult = await photoService.UpdateProductImagesAsync(id, productDto.Images);
                    if (!imageUpdateResult.IsSuccess)
                    {
                        return new Result<Product> { IsSuccess = false, Message = "Failed to update product images." };
                    }
                }

                await transaction.CommitAsync();
                return productUpdateResult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new Result<Product> { IsSuccess = false, Message = ex.Message };
            }
        }

        private async Task<Result<Product>> UpdateProductAsync(Guid id, CreateUpdateProductDto productDto)
        {
            try
            {
                var product = await unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return new Result<Product> { IsSuccess = false, Message = "Product not found." };
                }

                var slug = SlugHelper.GenerateSlug(productDto.Name);
                if (await unitOfWork.Products.IsSlugExits(slug))
                {
                    return new Result<Product> { IsSuccess = false, Message = "Slug already exists." };
                }

                mapper.Map(productDto, product);
                product.Slug = slug;
                var cachKey = $"Product_{id}";
                var cache = await cacheServices.GetCachedDataAsync<Product>(cachKey);
                if (cache != null)
                {
                    await cacheServices.RemoveCachedDataAsync(cachKey);
                }

                unitOfWork.Products.Update(product);
                await unitOfWork.CompleteAsync();

                return new Result<Product> { IsSuccess = true, Data = product };
            }
            catch (Exception ex)
            {
                return new Result<Product> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await unitOfWork.Products.GetProductWithImagesByIdAsync(id);
                if (product == null)
                {
                    return false;
                }

                if (product.Images != null && product.Images.Count != 0)
                {
                    foreach (var image in product.Images)
                    {
                        var publicId = PhotoExtensions.ExtractPublicId(image.ImageUrl);
                        await photoService.DeletePhotoAsync("product", publicId);
                        unitOfWork.Images.Delete(image);
                    }
                }

                var cacheKey = $"Product_{id}";
                var cache = await cacheServices.GetCachedDataAsync<Product>(cacheKey);
                if (cache != null)
                {
                    await cacheServices.RemoveCachedDataAsync(cacheKey);
                }

                unitOfWork.Products.Delete(product);

                var result = await unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return result > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<PagedResult<ProductInListDto>> GetAllProductsAsync(int page, int pageSize, string search,
            bool isDescending = false)
        {
            try
            {
                var productsQuery = await unitOfWork.Products.GetProductsWithImagesAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    productsQuery =
                        productsQuery.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                productsQuery = isDescending
                    ? productsQuery.OrderByDescending(p => p.Name)
                    : productsQuery.OrderBy(p => p.Name);

                var enumerable = productsQuery as Product[] ?? productsQuery.ToArray();
                var totalRows = enumerable.Count();

                var pagedProducts = enumerable
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
                logger.LogError(ex, "An error occurred while getting products.");
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

        public async Task<PagedResult<ProductDto>> GetProductByCategoryAsync(Guid categoryId, int page, int pageSize,
            bool isDescending)
        {
            try
            {
                var products = await unitOfWork.Products.GetByCategoryIdAsync(categoryId);
                products = isDescending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name);
                var totalRows = products.Count();
                var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PagedResult<ProductDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = mapper.Map<IEnumerable<ProductDto>>(pagedProducts),
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred while creating the order: {ErrorMessage}", ex.Message);
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
                var cacheResult = await cacheServices.GetCachedDataAsync<ProductDto>(cacheKey);
                if (cacheResult != null)
                {
                    return new Result<ProductDto>
                    {
                        IsSuccess = true,
                        Data = cacheResult
                    };
                }

                var product = await unitOfWork.Products.GetProductWithImagesByIdAsync(id);
                var productDto = mapper.Map<ProductDto>(product);
                await cacheServices.SetCachedDataAsync(cacheKey, productDto);
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

        public async Task<PagedResult<ProductInListDto>> SearchProductByNameAsync(string name, int page, int pageSize,
            bool isDescending)
        {
            try
            {
                var products = await unitOfWork.Products.SearchByNameAsync(name);
                products = isDescending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name);
                var totalRows = products.Count();
                var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PagedResult<ProductInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = mapper.Map<IEnumerable<ProductInListDto>>(pagedProducts),
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
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

        public async Task<PagedResult<ProductInListDto>> GetTopProductsAsync(int topCount,
            ProductSortCriteria sortCriteria, bool isDescending)
        {
            try
            {
                var products = await unitOfWork.Products.GetAllAsync();

                var productsQuery = products.AsQueryable();

                var sortExpressions =
                    new Dictionary<ProductSortCriteria, Func<IQueryable<Product>, IOrderedQueryable<Product>>>
                    {
                        {
                            ProductSortCriteria.CreatedDate,
                            q => isDescending ? q.OrderByDescending(p => p.DateCreated) : q.OrderBy(p => p.DateCreated)
                        },
                        {
                            ProductSortCriteria.ViewCount,
                            q => isDescending ? q.OrderByDescending(p => p.ViewCount) : q.OrderBy(p => p.ViewCount)
                        },
                        {
                            ProductSortCriteria.Name,
                            q => isDescending ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name)
                        }
                    };

                if (sortExpressions.TryGetValue(sortCriteria, out var sortExpression))
                {
                    productsQuery = sortExpression(productsQuery);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(sortCriteria), sortCriteria, null);
                }

                var topProducts = productsQuery.Take(topCount).ToList();

                var pagedResult = new PagedResult<ProductInListDto>
                {
                    IsSuccess = true,
                    Results = mapper.Map<List<ProductInListDto>>(topProducts),
                };
                return pagedResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var pagedResult = new PagedResult<ProductInListDto>
                {
                    IsSuccess = false,
                    PageSize = topCount,
                    CurrentPage = 0,
                    AdditionalData = e.Message
                };
                return pagedResult;
            }
        }

        public async Task<PagedResult<ProductInListDto>> GetProductBySupplier(Guid supplierId, int page, int pageSize,
            bool isDescending)
        {
            try
            {
                var products = await unitOfWork.Products.GetAllAsync();
                products = products.Where(p => p.SupplierId == supplierId);
                products = isDescending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name);
                var totalRows = products.Count();
                var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PagedResult<ProductInListDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = mapper.Map<IEnumerable<ProductInListDto>>(pagedProducts),
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                return new PagedResult<ProductInListDto>()
                {
                    CurrentPage = page,
                    PageSize = 1,
                    RowCount = 0,
                    Results = [],
                    IsSuccess = false,
                    AdditionalData = e.Message
                };
            }
        }
        public async Task<ResultBase> UpdateProductRating(Guid productId, int reviewRate, int reviewCount)
        {
            try
            {
                var cacheKey = $"Product_{productId}";
                var productInCache = await cacheServices.GetCachedDataAsync<ProductDto>(cacheKey);
                var product = (mapper.Map<Product>(productInCache)
                    ?? await unitOfWork.Products.GetByIdAsync(productId))
                    ?? throw new Exception("Product not found");
                product.Rating = product.Rating + reviewRate;
                product.RatingCount = product.RatingCount + reviewCount;
                unitOfWork.Products.Update(product);
                await unitOfWork.CompleteAsync();
                await cacheServices.RemoveCachedDataAsync(cacheKey);
                return new ResultBase
                {
                    IsSuccess = true,
                };

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new ResultBase
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }

        }
        public async Task<ResultBase> UpdateProductQuantity(Guid productId, int quantity)
        {
            try
            {
                var cacheKey = $"Product_{productId}";
                var productInCache = await cacheServices.GetCachedDataAsync<ProductDto>(cacheKey);
                var product = (mapper.Map<Product>(productInCache)
                    ?? await unitOfWork.Products.GetByIdAsync(productId))
                    ?? throw new Exception("Product not found");
                product.Quantity = product.Quantity - quantity;
                product.UnitSold = product.UnitSold.HasValue ? product.UnitSold + quantity : quantity;
                unitOfWork.Products.Update(product);
                await unitOfWork.CompleteAsync();
                await cacheServices.RemoveCachedDataAsync(cacheKey);
                return new ResultBase
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new ResultBase
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}