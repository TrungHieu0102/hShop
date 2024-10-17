using Application.DTOs.CategoriesDto;
using Core.Entities;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<CategoryDto>> GetByIdAsync(Guid id); 
        Task<PagedResult<CategoryDto>> GetAllAsync(int page, int pageSize, string search, bool IsDecsending = false);
        Task<int>AddCategoryAsync(CreateUpdateCategoryDto categoryDto);
        Task<Result<Category>> UpdateCategoryAsync(Guid id, CreateUpdateCategoryDto categoryDto);
        Task<bool> DeleteCategoryAsync(Guid id);  
    }
}
