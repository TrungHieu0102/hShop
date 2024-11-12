using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class WishlistRepository : RepositoryBase<Wishlist, Guid>, IWishlistRepository
    {
        public WishlistRepository(HshopContext dbContext) : base(dbContext)
        {
          
        }
        public async Task<bool> CheckExists(Guid userId, Guid productId)
        {
            var result = await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.ProductId == productId);
            return result;
        }
        public async Task<Wishlist?> GetWishlist(Guid userId, Guid productId)
        {
            var result = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            return result;
        }
        public async Task<IEnumerable<Wishlist>> GetWishListByUserId(Guid userId)
        {
            return await _context.Wishlists.Where(w => w.UserId == userId).ToListAsync();
        }

    }
}
