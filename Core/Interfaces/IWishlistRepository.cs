using Core.Entities;

namespace Core.Interfaces
{
    public interface IWishlistRepository : IRepositoryBase <Wishlist,Guid>
    {
        Task<bool> CheckExists(Guid userId, Guid productId);
        Task<Wishlist?> GetWishlist(Guid userId, Guid productId);
        Task<IEnumerable<Wishlist>> GetWishListByUserId(Guid userId);
    }
}
