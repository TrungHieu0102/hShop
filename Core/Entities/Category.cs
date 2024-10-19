
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(100)] 
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(120)] 
        [Required(ErrorMessage = "Slug là bắt buộc")]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)] 
        public string? Description { get; set; }

        [MaxLength(300)] 
        [Url(ErrorMessage = "URL không hợp lệ")] 
        public string? PictureUrl { get; set; }

    }
}
