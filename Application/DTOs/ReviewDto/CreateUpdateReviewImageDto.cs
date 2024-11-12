using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ReviewDto
{
    public class CreateUpdateReviewImage
    {
		[Key]
		public Guid Id { get; set; }
		[Required]
		public Guid ReviewId { get; set; }
		[Required]
		[StringLength(500)]
		public string ImageUrl { get; set; }

	}
}
