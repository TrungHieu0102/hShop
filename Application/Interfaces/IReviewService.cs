using Application.DTOs.ReviewDto;
using Core.Model;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<Result<ReviewDto>> CreateReview(CreateUpdateReviewDto reviewRequest);
        Task<Result<ReviewDto>> GetReviewById(Guid Id);
        Task<PagedResult<ReviewDto>> GetAllReviewByProductId(Guid Id, int page, int pageSize, bool isDescending);
        Task<Result<ReviewDto>> UpdateReviewWithImages(Guid id, CreateUpdateReviewDto request);
        Task<Result<ReviewDto>> DeleteReviewByUser(Guid id, Guid userId);
        Task<Result<ReviewDto>> DeleteReviewById(Guid id);
    }
}
