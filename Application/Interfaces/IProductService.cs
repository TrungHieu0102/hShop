﻿using Application.DTOs;
using Core.Entities;
using Core.Model;


namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductDto>> GetProductByIdAsync(Guid id); 
        Task<int> AddProductAsync(CreateUpdateProductDto productDto);
        Task<Result<Product>> UpdateProductAsync(Guid id, CreateUpdateProductDto productDto);
        Task<bool> DeleteProductAsync(Guid id);
        Task<PagedResult<ProductDto>> GetAllProductsAsync(int page, int pageSize, string search, bool IsDecsending);
    }
}
