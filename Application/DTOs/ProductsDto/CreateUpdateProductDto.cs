using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ProductsDto
{
    public class CreateUpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Description { get; set; } = string.Empty;
        [Range(0,10000, ErrorMessage = "Price must be between 0 and 10000.")]
        public int ? Quantity { get; set; }
        [Required(ErrorMessage = "Unit is required.")]
        [MaxLength(20, ErrorMessage = "Unit cannot exceed 20 characters.")]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Supplier is required.")]
        public Guid SupplierId { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal? Discount { get; set; }

        // Allow uploading up to 10 images
        [MaxLength(10, ErrorMessage = "You can upload a maximum of 10 images.")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
