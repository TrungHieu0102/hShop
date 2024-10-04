using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;
;

namespace Infrastructure.Data
{
    public class SupplierRepository : RepositoryBase<Supplier>, ISupplierRepository
    {
        public SupplierRepository(HshopContext context) : base(context)
        {

        }
    }
}
