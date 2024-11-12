using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ReviewImage
    {
        [Key]
        public Guid Id { get; set; } 

        [Required]
        public Guid ReviewId { get; set; } 

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } 

        // Navigation property
        [ForeignKey("ReviewId")]
        public virtual Review Review { get; set; } 
    }
}
