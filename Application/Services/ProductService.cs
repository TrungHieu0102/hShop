using Application.DTOs.ProductsDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Model;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWorkBase unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            _unitOfWork.Products.Add(product);
            return await _unitOfWork.CompleteAsync();
        }
        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                _unitOfWork.Products.Delete(product);
                var result = await _unitOfWork.CompleteAsync();
                return result > 0;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        public async Task<PagedResult<ProductDto>> GetAllProductsAsync(int page, int pageSize, string search, bool IsDecsending = false)
        {
            var products = await _unitOfWork.Products.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

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
                var product = await _unitOfWork.Products.GetByIdAsync(id);
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

        public async Task<Result<Product>> UpdateProductAsync(Guid id, CreateUpdateProductDto productDto)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                var slug = SlugHelper.GenerateSlug(productDto.Name);
                if (await _unitOfWork.Products.IsSlugExits(slug))
                {
                    return new Result<Product>
                    {
                        IsSuccess = false,
                        Message = "Slug already exists."
                    };
                }
                _mapper.Map(productDto, product);

                product.Slug = slug;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();
                return new Result<Product>
                {
                    IsSuccess = true
                };
            }
            catch (InvalidOperationException)
            {
                return new Result<Product>
                {
                    IsSuccess = false,
                    Message = "Product not found."
                };
            }
        }
    }
}
