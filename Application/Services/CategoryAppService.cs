using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class CategoryAppService : ICategoryAppService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkBase _unitOfWork;
        public CategoryAppService(ICategoryRepository categoryRepository, IMapper mapper, IUnitOfWorkBase unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddCategoryAsync(CategoryDto categoryDto)
        {
            var product = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            await _categoryRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
           var category = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(category);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
           var category = await _categoryRepository.GetByIdAsync(id);
            return _mapper.Map<CategoryDto>(category);  
        }

        public async Task UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
