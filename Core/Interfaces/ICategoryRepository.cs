using Core.Entities;

namespace Core.Interfaces
{
    public interface ICategoryRepository : IRepositoryBase<Category, Guid>
    {
        Task<Category> GetBySlug(string slug);
        Task<bool> IsSlugExits(string slug);
    }
}
