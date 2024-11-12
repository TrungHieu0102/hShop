using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; } 

        [Required]
        public Guid UserId { get; set; } 

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } 

        [Range(1, 5)]
        public int Rating { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } 

        [ForeignKey("UserId")]
        public virtual User User { get; set; } 
        public virtual ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();

    }
}
