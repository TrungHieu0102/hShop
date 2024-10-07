using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;
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
    }
}
