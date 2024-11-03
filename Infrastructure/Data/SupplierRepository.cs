using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;


namespace Infrastructure.Data
{
    public class SupplierRepository(HshopContext context, IMapper mapper)
        : RepositoryBase<Supplier, Guid>(context), ISupplierRepository
    {
        private readonly IMapper _mapper = mapper;
    }
}
