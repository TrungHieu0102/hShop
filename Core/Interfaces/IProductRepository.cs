
namespace Core.Interfaces
{
    public interface IProductRepository : IRepositoryBase<Product,Guid>
    {
        Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetProductsWithImagesAsync();    
        Task<IEnumerable<Product>> SearchByNameAsync(string name);
        Task<Product> GetBySlug(string slug);
        Task<bool> IsSlugExits(string slug);
        Task<Product> GetProductWithImagesByIdAsync(Guid id);

    }
}
