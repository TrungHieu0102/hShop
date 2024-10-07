
namespace Core.Interfaces
{
    public interface IUnitOfWorkBase
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ISupplierRepository Suppliers { get; }  
        Task<int> CompleteAsync();
    }

}
