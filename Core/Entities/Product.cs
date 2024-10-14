
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.Entities
{
    [Table("Products")]
    [Index(nameof(Slug), IsUnique = true)]
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;
        [MaxLength(120)]
        [Required]
        public string Slug { get; set; } = string.Empty;
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;
        [MaxLength(20)]
        [Required]
        public decimal Price { get; set; }
        [MaxLength(20)]
        [Required]
        public string Unit { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public decimal? Discount { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int? ViewCount { get; set; }  
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }

    }
}
