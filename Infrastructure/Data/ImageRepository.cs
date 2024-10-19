using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ImageRepository : RepositoryBase<ProductImage, Guid>, IImageRepository
    {
        public ImageRepository(HshopContext context) : base(context)
        {

        }
        public async Task<List<ProductImage>> GetImagesByProductIdAsync(Guid productId)
        {
            return await _context.ProductImages
                .Where(img => img.ProductId == productId)
                .ToListAsync();
        }

        public async Task DeleteImagesByProductIdAsync(Guid productId)
        {
            var images = await GetImagesByProductIdAsync(productId);

            if (images.Any())
            {
                _context.ProductImages.RemoveRange(images);
            }
        }
    }
}
