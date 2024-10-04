using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;


namespace Infrastructure.Data
{
    public class ProductRepository: RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(HshopContext context) : base(context)
        {

        }
    }
}
