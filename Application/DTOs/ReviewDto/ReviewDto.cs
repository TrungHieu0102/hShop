using Core.Entities;

namespace Application.DTOs.ReviewDto
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<ReviewImageDto> ReviewImages { get; set; } = new List<ReviewImageDto>();
    }
}
