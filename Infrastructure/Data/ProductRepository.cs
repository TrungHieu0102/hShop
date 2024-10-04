using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ProductRepository: RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(HshopContext context) : base(context)
        {
        }
    }
}
