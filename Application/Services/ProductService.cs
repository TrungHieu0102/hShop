using Application.DTOs.ProductsDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public ProductService(IUnitOfWorkBase unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;

        }
        public async Task<int> AddProductAsync(CreateUpdateProductDto productDto)
        {
            var slug = SlugHelper.GenerateSlug(productDto.Name);
            if (await _unitOfWork.Products.IsSlugExits(slug))
            {
                throw new InvalidOperationException("Slug already exists.");
            }

            var product = _mapper.Map<Product>(productDto);
            product.Slug = slug;
            product.Id = Guid.NewGuid();
            product.Images = new List<ProductImage>();
            if (productDto.Images != null && productDto.Images.Any())
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
            return await _unitOfWork.CompleteAsync();
        }
        public async Task<Result<Product>> UpdateProductWithImagesAsync(Guid id, CreateUpdateProductDto productDto)
        {
            var productUpdateResult = await UpdateProductAsync(id, productDto);
            if (!productUpdateResult.IsSuccess)
            {
                return productUpdateResult;
            }
            if (productDto.Images != null && productDto.Images.Any())
            {
                var imageUpdateResult = await _photoService.UpdateProductImagesAsync(id, productDto.Images);
                if (!imageUpdateResult.IsSuccess)
                {
                    return new Result<Product> { IsSuccess = false, Message = "Failed to update product images." };
                }
            }
            return productUpdateResult;
        }

        public async Task<Result<Product>> UpdateProductAsync(Guid id, CreateUpdateProductDto productDto)
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

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            return new Result<Product> { IsSuccess = true, Data = product };
        }
        public async Task<bool> DeleteProductAsync(Guid id)
        {
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
                        var publicId = ExtractPublicId(image.ImageUrl);
                        await _photoService.DeletePhotoAsync("product", publicId);
                        _unitOfWork.Images.Delete(image);
                    }
                }
                _unitOfWork.Products.Delete(product);
                var result = await _unitOfWork.CompleteAsync();

                return result > 0;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public async Task<PagedResult<ProductInListDto>> GetAllProductsAsync(int page, int pageSize, string search, bool IsDecsending = false)
        {
            var products = await _unitOfWork.Products.GetProductsWithImagesAsync();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            products = IsDecsending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name);

            var totalRows = products.Count();

            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<ProductInListDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalRows,
                Results = _mapper.Map<IEnumerable<ProductInListDto>>(pagedProducts)
            };
        }
        public async Task<PagedResult<ProductDto>> GetProductByCategoryAsync(Guid categoryId, int page, int pageSize, bool IsDecsending)
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
                Results = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts)
            };
        }
        public async Task<Result<ProductDto>> GetProductByIdAsync(Guid id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetProductWithImagesByIdAsync(id);
                var productDto = _mapper.Map<ProductDto>(product);
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
            var products = await _unitOfWork.Products.SearchByNameAsync(name);
            products = IsDecsending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name);
            var totalRows = products.Count();
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<ProductInListDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalRows,
                Results = _mapper.Map<IEnumerable<ProductInListDto>>(pagedProducts)
            };
        }

        public string ExtractPublicId(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentException("Image URL cannot be null or empty.", nameof(imageUrl));
            }

            var uri = new Uri(imageUrl);
            var segments = uri.Segments;
            if (segments.Length < 3)
            {
                throw new InvalidOperationException("Invalid image URL format.");
            }
            var publicIdWithVersion = segments[segments.Length - 1]; // phần cuối cùng là tên file
            var publicId = Path.GetFileNameWithoutExtension(publicIdWithVersion);

            return publicId;
        }

    }
}
