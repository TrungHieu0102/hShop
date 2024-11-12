using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.ReviewDto
{
    public class CreateUpdateReviewDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        [StringLength(1000)]
        public string Content { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public virtual ICollection<IFormFile> ReviewImages { get; set; } = new List<IFormFile>();
    }
}
