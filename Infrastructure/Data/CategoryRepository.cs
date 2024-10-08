using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Infrastructure.Data
{
    public class CategoryRepository : RepositoryBase<Category, Guid>, ICategoryRepository
    {
        private readonly IMapper _mapper;
        public CategoryRepository(HshopContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;   
        }

        public async Task<Category> GetBySlug(string slug)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == slug);
            if(category == null) throw new Exception($"Not found {slug}");
            return category;
        }

        public async Task<bool> IsSlugExits(string slug)
        {
            return await _context.Categories.AnyAsync(c => c.Slug == slug);
          
        }
    }
}
