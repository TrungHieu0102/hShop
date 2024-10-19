using Core.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ProductRepository : RepositoryBase<Product, Guid>, IProductRepository
    {

        public ProductRepository(HshopContext context) : base(context)
        {

        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
        {
            var product = await _context.Products.Where(p => p.CategoryId == categoryId).Include(p => p.Images).ToListAsync();
            return product;
        }

        public async Task<Product> GetBySlug(string slug)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Slug == slug);
            if (product == null) throw new Exception($"Not found {slug}");
            return product;
        }
        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            return await _context.Products
            .Where(p => p.Name.Contains(name)).Include(p => p.Images)
            .ToListAsync();
        }
        public async Task<bool> IsSlugExits(string slug)
        {
            return await _context.Products.AnyAsync(p => p.Slug == slug);
        }

        public async Task<Product> GetProductWithImagesByIdAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Products
        .Include(p => p.Images)
        .FirstOrDefaultAsync(p => p.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<Product>> GetProductsWithImagesAsync()
        {
            return await _context.Products
        .Include(p => p.Images)
        .ToListAsync();
        }
    }
}
