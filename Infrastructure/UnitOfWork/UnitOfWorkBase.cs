using AutoMapper;
using Core.Interfaces;
using Infrastructure.Data;


namespace Infrastructure.UnitOfWork
{
    public class UnitOfWorkBase : IUnitOfWorkBase
    {
        private readonly HshopContext _context;
        public IProductRepository Products { get; set; }
        public ICategoryRepository Categories { get; set; }
        public ISupplierRepository Suppliers { get; set; }

        public UnitOfWorkBase(HshopContext context, IMapper mapper)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context, mapper);
            Suppliers = new SupplierRepository(_context, mapper);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


    }

}
