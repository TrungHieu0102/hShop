using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Model;
namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWorkBase unitOfWork, IMapper mapper) => (_unitOfWork, _mapper) = (unitOfWork, mapper);

        public async Task<int> AddCategoryAsync(CreateUpdateCategoryDto categoryDto)
        {
            var slug = SlugHelper.GenerateSlug(categoryDto.Name);
            if (await _unitOfWork.Categories.IsSlugExits(slug))
            {
                throw new InvalidOperationException("Slug already exists.");
            }
            var category = _mapper.Map<Category>(categoryDto);
            category.Slug = slug;
            category.Id = Guid.NewGuid();
            _unitOfWork.Categories.Add(category);
            return await _unitOfWork.CompleteAsync();
        }
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                _unitOfWork.Categories.Delete(category);
                var result = await _unitOfWork.CompleteAsync();
                return result > 0;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        public async Task<PagedResult<CategoryDto>> GetAllAsync(int page, int pageSize, string search, bool IsDecsending = false)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                categories = categories.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            categories = IsDecsending ? categories.OrderByDescending(p => p.Name) : categories.OrderBy(p => p.Name);

            var totalRows = categories.Count();

            var pagedCategories = categories.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<CategoryDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalRows,
                Results = _mapper.Map<IEnumerable<CategoryDto>>(pagedCategories)
            };
        }
        public async Task<Result<CategoryDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
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
                _mapper.Map(categoryDto, category);
                category.Slug = slug;
                _unitOfWork.Categories.Update(category);
                await _unitOfWork.CompleteAsync();
                return new Result<Category>
                {
                    IsSuccess = true
                };
            }
            catch (InvalidOperationException)
            {
                return new Result<Category>
                {
                    IsSuccess = false,
                    Message = "Category not found."
                };
            }
        }
    }
}


