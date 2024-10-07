using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;


namespace Infrastructure.Data
{
    public class SupplierRepository : RepositoryBase<Supplier, Guid>, ISupplierRepository
    {
        private readonly IMapper _mapper;

        public SupplierRepository(HshopContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
    }
}
