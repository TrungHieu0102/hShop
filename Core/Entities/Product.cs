

namespace Core.Entities
{
    public class Product
    {
        public Guid Id { get; set; }    
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } = string.Empty;
        public decimal? Discount { get; set; }
        public DateOnly DateCreated { get; set; }
        public int ViewCount { get; set; }  
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }

    }
}
