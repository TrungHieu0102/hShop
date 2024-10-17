using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ProductsDto
{
    public class ProductDto
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(120, ErrorMessage = "Slug cannot exceed 120 characters.")]
        [Required(ErrorMessage = "Slug is required.")]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [MaxLength(20, ErrorMessage = "Unit cannot exceed 20 characters.")]
        [Required(ErrorMessage = "Unit is required.")]
        public string Unit { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal? Discount { get; set; }

        [Required(ErrorMessage = "Date created is required.")]
        public DateTime DateCreated { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "View count must be a positive value.")]
        public int? ViewCount { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Supplier ID is required.")]
        public Guid SupplierId { get; set; }

        // Danh sách các ảnh của sản phẩm
        [MaxLength(10, ErrorMessage = "A product can have a maximum of 10 images.")]
        public ICollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();

    }
}
