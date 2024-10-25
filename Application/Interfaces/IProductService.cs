﻿using Application.DTOs.ProductsDto;
using Core.Entities;
using Core.Model;


namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductDto>> GetProductByIdAsync(Guid id); 
        Task<bool> AddProductAsync(CreateUpdateProductDto productDto);
        Task<Result<Product>> UpdateProductWithImagesAsync(Guid id, CreateUpdateProductDto productDto);
        Task<bool> DeleteProductAsync(Guid id);
        Task<PagedResult<ProductInListDto>> GetAllProductsAsync(int page, int pageSize, string search, bool IsDecsending);
        Task<PagedResult<ProductDto>> GetProductByCategoryAsync(Guid categoryId, int page, int pageSize, bool IsDecsending);
        Task<PagedResult<ProductInListDto>> SearchProductByNameAsync(string name, int page, int pageSize, bool IsDecsending);
    }
}
