using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [Required(ErrorMessage = "Image URL is required.")]
        [MaxLength(255, ErrorMessage = "Image URL cannot exceed 255 characters.")]
        public string ImageUrl { get; set; }
        [Required]
        public bool IsPrimary { get; set; }
        [Required(ErrorMessage = "Image type is required.")]
        [MaxLength(50, ErrorMessage = "Image type cannot exceed 50 characters.")]
        public string ImageType { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
