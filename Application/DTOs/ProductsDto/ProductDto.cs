using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ProductsDto
{
    public class ProductDto
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? Discount { get; set; }
        public int? Quantity { get; set; }
        public DateTime DateCreated { get; set; }
        public int? ViewCount { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SupplierId { get; set; }
        public ICollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }
}
