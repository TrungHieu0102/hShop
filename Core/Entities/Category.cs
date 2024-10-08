
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;
        [MaxLength(120)]
        [Required]
        public string Slug { get; set; } = string.Empty;
        [MaxLength(500)]
        public string ?Description { get; set; }
        [MaxLength(300)]
        public string? PictureUrl { get; set; } 

    }
}
