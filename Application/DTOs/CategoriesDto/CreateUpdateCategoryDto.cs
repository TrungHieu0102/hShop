using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace Application.DTOs.CategoriesDto
{
    public class CreateUpdateCategoryDto
    {
        [MaxLength(100)]
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public IFormFile? Images { get; set; }

    }
}
