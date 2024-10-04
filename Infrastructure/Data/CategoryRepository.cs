using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;


namespace Infrastructure.Data
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(HshopContext context) : base(context)
        {

        }
    }
}
