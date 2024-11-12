using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    internal class ReviewImageRepository(HshopContext context) : RepositoryBase<ReviewImage, Guid>(context), IReviewImageRepository
    {
        
        public async Task<List<ReviewImage>> GetImagesByReviewIdAsync(Guid reviewId)
        {
            return await _context.ReviewImages
                .Where(img => img.ReviewId == reviewId)
                .ToListAsync();
        }

        public async Task DeleteImagesByReviewIdAsync(Guid reviewId)
        {
            var images = await GetImagesByReviewIdAsync(reviewId);

            if (images.Any())
            {
                _context.ReviewImages.RemoveRange(images);
            }
        }
    }
}
