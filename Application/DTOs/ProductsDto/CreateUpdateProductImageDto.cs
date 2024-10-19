using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductsDto
{
    public class CreateUpdateProductImageDto
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [Required(ErrorMessage = "Image URL is required.")]
        [MaxLength(255, ErrorMessage = "Image URL cannot exceed 255 characters.")]
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        [MaxLength(50, ErrorMessage = "Image type cannot exceed 50 characters.")]
        public string ImageType { get; set; } = string.Empty;
        [Required(ErrorMessage = "Created date is required.")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
