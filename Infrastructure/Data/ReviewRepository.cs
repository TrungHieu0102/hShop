using Application.DTOs.ReviewDto;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class ReviewRepository : RepositoryBase<Review, Guid>, IReviewRepository
    {
        public ReviewRepository(HshopContext context) : base(context)
        {

        }
        public async Task<IEnumerable<Review>> GetReviewByProductId(Guid id)
        {
            var reviews = await _context.Reviews.Where(r => r.ProductId == id).Include(r => r.ReviewImages).ToListAsync();
            return reviews;
        }

        public async Task<List<Review>> GetReviewByUserId(Guid userId)
        {
           var review = await _context.Reviews.Where(r => r.UserId == userId).Include(r => r.ReviewImages).ToListAsync();
            return review;
        }

        public async Task<bool> HasReview(Guid productId, Guid userId)
        {
            return await _context.Reviews.AnyAsync(r => r.ProductId == productId && r.UserId == userId);
        }
    }
}
