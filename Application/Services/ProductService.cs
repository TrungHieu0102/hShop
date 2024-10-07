using Application.DTOs;
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
            if(await _unitOfWork.Products.IsSlugExits(slug))
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
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                _unitOfWork.Products.Delete(product);
                var result = await _unitOfWork.CompleteAsync();
                return result > 0;
            }
            return false; 
        }
        public async Task<PagedResult<ProductDto>> GetAllProductsAsync(int page, int pageSize, string search, bool IsDecsending = false )
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

        public async Task<ProductDto> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductAsync(Guid id, CreateUpdateProductDto productDto)
        {      
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new InvalidOperationException("Product not found.");
            }
            var slug = SlugHelper.GenerateSlug(productDto.Name);

            if (await _unitOfWork.Products.IsSlugExits(slug))
            {
                throw new InvalidOperationException("Slug already exists for another product.");
            }
            _mapper.Map(productDto, existingProduct);
            existingProduct.Slug = slug;
            _unitOfWork.Products.Update(existingProduct);
            await _unitOfWork.CompleteAsync();
        }
    }
}
