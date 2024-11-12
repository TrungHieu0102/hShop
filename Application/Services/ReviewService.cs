using Application.DTOs.ReviewDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ReviewService(IUnitOfWorkBase unitOfWork, IPhotoService photoService, IMapper mapper, ILogger<ReviewService> logger, ICacheService cacheService, IProductService productService) : IReviewService
    {
        private readonly IUnitOfWorkBase _unitOfWork = unitOfWork;
        private readonly IPhotoService _photoService = photoService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ReviewService> _logger = logger;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IProductService _productService = productService;

        /// <summary>
        /// Creates a new product review for a user and updates the product rating based on the new review.
        /// This method performs several key actions:
        /// 1. Checks if the user has already reviewed the product, and throws an exception if they have.
        /// 2. Verifies that the user has ordered the product before allowing them to review it.
        /// 3. Maps the review request data to a new Review entity and generates a unique ID.
        /// 4. If there are images included with the review, uploads each image, retrieves the secure URL, and
        ///    associates the images with the review.
        /// 5. Updates the product's rating based on the new review rating.
        /// 6. Adds the review to the repository and saves changes within a transaction context.
        /// 7. Commits the transaction if all operations are successful; otherwise, rolls back in case of any errors.
        ///
        /// In case of failure, logs the exception, rolls back the transaction, and returns a failure result with an error message.
        /// </summary>
        /// <param name="reviewRequest">The data for creating or updating a review, including product and user IDs, rating, and optional images.</param>
        /// <returns>A Result object containing the created ReviewDto on success, or an error message on failure.</returns>
        public async Task<Result<ReviewDto>> CreateReview(CreateUpdateReviewDto reviewRequest)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
               
                if (await _unitOfWork.Reviews.HasReview(reviewRequest.ProductId, reviewRequest.UserId))
                {
                    throw new Exception("You have already reviewed this product");
                }
                var userOrder = await _unitOfWork.Orders.GetOrderByUserId(reviewRequest.UserId)
                                   ?? throw new Exception("You have not ordered this product");

                var isOrdered = userOrder.Any(item => item.OrderDetails.Any(detail => detail.ProductId == reviewRequest.ProductId));
                if (!isOrdered)
                {
                    throw new Exception("You have not ordered this product");
                }
                var review = _mapper.Map<Review>(reviewRequest);
                review.Id = Guid.NewGuid();

                if (reviewRequest.ReviewImages != null && reviewRequest.ReviewImages.Count > 0)
                {
                    foreach (var image in reviewRequest.ReviewImages)
                    {
                        var imageUploadResult = await _photoService.AddPhotoAsync(image, "review");
                        if (imageUploadResult.Error != null)
                        {
                            throw new Exception(imageUploadResult.Error.Message);
                        }

                        review.ReviewImages.Add(new ReviewImage
                        {
                            Id = Guid.NewGuid(),
                            ImageUrl = imageUploadResult.SecureUrl.AbsoluteUri,
                            ReviewId = review.Id
                        });
                    }
                }
                var reviewDto = _mapper.Map<ReviewDto>(review);
                var result = await _productService.UpdateProductRating(reviewRequest.ProductId, reviewRequest.Rating, 1);
                if (!result.IsSuccess)
                {
                    throw new Exception(result.Message);
                }
                _unitOfWork.Reviews.Add(review);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return new Result<ReviewDto>
                {
                    IsSuccess = true,
                    Data = reviewDto
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await transaction.RollbackAsync();
                return new Result<ReviewDto> { IsSuccess = false, Message = ex.Message };
            }
        }
        /// <summary>
        /// Retrieves a specific review by its unique identifier, including any associated images.
        /// Checks for cached data first to improve performance; if not found, fetches from the database and updates the cache.
        /// </summary>
        /// <param name="Id">The unique identifier of the review to retrieve.</param>
        /// <returns>A Result object containing the review data in ReviewDto format, or an error message if not found.</returns>
        public async Task<Result<ReviewDto>> GetReviewById(Guid Id)
        {
            try
            {
                var cacheKey = $"Review-{Id}";
                var reviewInCache = await _cacheService.GetCachedDataAsync<ReviewDto>(cacheKey);
                var review = ((_mapper.Map<Review>(reviewInCache) ?? await _unitOfWork.Reviews.GetByIdAsync(Id)) ?? null) ?? throw new Exception("Review not found");
                await _cacheService.SetCachedDataAsync(cacheKey, _mapper.Map<ReviewDto>(review));
                return new Result<ReviewDto>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ReviewDto>(review)
                };
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex.Message);
                return new Result<ReviewDto> { IsSuccess = false, Message = ex.Message };
            }
        }
        /// <summary>
        /// Retrieves all reviews for a specific product, supports pagination and ordering by creation date.
        /// Maps review data, including images, into DTOs for a paginated result.
        /// </summary>
        /// <param name="Id">The unique identifier of the product for which reviews are being retrieved.</param>
        /// <param name="page">The current page number for pagination.</param>
        /// <param name="pageSize">The number of reviews per page.</param>
        /// <param name="isDescending">Indicates if the reviews should be sorted in descending order by creation date.</param>
        /// <returns>A PagedResult object containing a list of ReviewDto objects for the requested page, or an error message.</returns>
        public async Task<PagedResult<ReviewDto>> GetAllReviewByProductId(Guid Id, int page, int pageSize, bool isDescending)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetReviewByProductId(Id);
                reviews = isDescending ? reviews.OrderByDescending(r => r.CreatedAt) : reviews.OrderBy(r => r.CreatedAt);
                var totalRows = reviews.Count();
                var result = reviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var reviewDtos = _mapper.Map<List<ReviewDto>>(result);
                return new PagedResult<ReviewDto>
                {
                    CurrentPage = page,
                    IsSuccess = true,
                    Results = reviewDtos,
                    PageSize = pageSize,
                    RowCount = totalRows,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new PagedResult<ReviewDto>
                {
                    CurrentPage = page,
                    IsSuccess = false,
                    Results = [],
                    PageSize = pageSize,
                    RowCount = 0,
                    AdditionalData = ex.Message
                };
            }
        }
        /// <summary>
        /// Updates an existing review and its images. Deletes old images if new ones are provided in the request.
        /// Handles updates within a transaction to ensure consistency and rolls back if any errors occur.
        /// </summary>
        /// <param name="id">The unique identifier of the review to update.</param>
        /// <param name="request">The updated review data, including new images if any.</param>
        /// <returns>A Result object containing the updated ReviewDto on success, or an error message on failure.</returns>
        public async Task<Result<ReviewDto>> UpdateReviewWithImages(Guid id, CreateUpdateReviewDto request)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var updateReviewResult = await UpdateReviewAsync(id, request);
                if (!updateReviewResult.IsSuccess)
                {
                    throw new Exception(updateReviewResult.Message);
                }
                if (request.ReviewImages.Count != 0)
                {
                    var updateReviewImageResult = await _photoService.UpdateReviewImage(id, request.ReviewImages);
                    if (!updateReviewImageResult.IsSuccess)
                    {
                        throw new Exception(updateReviewImageResult.Message);
                    }
                }

                await transaction.CommitAsync();
                return updateReviewResult;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await transaction.RollbackAsync();
                return new Result<ReviewDto> { IsSuccess = false, Message = ex.Message };


            }
        }
        /// <summary>
        /// Updates review details without modifying associated images and adjusts the product rating accordingly.
        /// Does not utilize transactions, as image management is handled separately.
        /// </summary>
        public async Task<Result<ReviewDto>> UpdateReviewAsync(Guid id, CreateUpdateReviewDto request)
        {
            try
            {
                var cacheKey = $"Review-{id}";
                var reviewInCache = await _cacheService.GetCachedDataAsync<ReviewDto>(cacheKey);
                var review = (_mapper.Map<Review>(reviewInCache) ?? await _unitOfWork.Reviews.GetByIdAsync(id)) ?? throw new Exception("Review not found");
                var ratetingUpdate = -(review.Rating - request.Rating);
                var result = await _productService.UpdateProductRating(request.ProductId, ratetingUpdate, 0);
                if (!result.IsSuccess)
                {
                    throw new Exception(result.Message);
                }
                _mapper.Map(request, review);
                _unitOfWork.Reviews.Update(review);

                await _unitOfWork.CompleteAsync();
                await _cacheService.RemoveCachedDataAsync(cacheKey);
                return new Result<ReviewDto>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ReviewDto>(review)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new Result<ReviewDto> { IsSuccess = false, Message = ex.Message };
            }
        }
        /// <summary>
        /// Deletes a review by its unique identifier, ensuring associated data and cache are updated.
        /// Checks if the review exists in the cache, then deletes it from both the database and cache.
        /// Updates the product rating to reflect the removal of this review. 
        /// Returns a success message if the deletion is successful, or an error message if an issue occurs.
        /// </summary>
        /// <param name="id">The unique identifier of the review to delete.</param>
        /// <returns>A Result object indicating whether the deletion was successful or not.</returns>
        public async Task<Result<ReviewDto>> DeleteReviewById(Guid id)
        {
            try
            {
                var cacheKey = $"Review-{id}";
                var reviewInCache = await _cacheService.GetCachedDataAsync<ReviewDto>(cacheKey);
                var review = (_mapper.Map<Review>(reviewInCache)
                    ?? await _unitOfWork.Reviews.GetByIdAsync(id))
                    ?? throw new Exception("Review not found");
                _unitOfWork.Reviews.Delete(review);
                await _unitOfWork.CompleteAsync();
                await _cacheService.RemoveCachedDataAsync(cacheKey);
                await _productService.UpdateProductRating(review.ProductId, -(review.Rating), -1);
                return new Result<ReviewDto>
                {
                    IsSuccess = true,
                    Message = "Review deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new Result<ReviewDto> { IsSuccess = false, Message = ex.Message };
            }
        }
        /// <summary>
        /// Deletes a review based on both review ID and user ID, typically used in authorized APIs.
        /// Retrieves the user's reviews by their ID, then deletes the review matching the specified review ID.
        /// Adjusts the product's rating after the review is deleted.
        /// Returns a success message if the review is found and deleted, or an error message if the review is not found.
        /// </summary>
        /// <param name="id">The unique identifier of the review to delete.</param>
        /// <param name="userId">The unique identifier of the user who owns the review.</param>
        /// <returns>A Result object indicating whether the deletion was successful or not.</returns>
        public async Task<Result<ReviewDto>> DeleteReviewByUser(Guid id, Guid userId)
        {
            var review = await _unitOfWork.Reviews.GetReviewByUserId(userId);
            var reviewToDelete = review.FirstOrDefault(r => r.Id == id);
            if (reviewToDelete == null)
            {
                return new Result<ReviewDto> { IsSuccess = false, Message = "Review not found" };
            }
            _unitOfWork.Reviews.Delete(reviewToDelete);
            await _unitOfWork.CompleteAsync();
            await _productService.UpdateProductRating(reviewToDelete.ProductId, -(reviewToDelete.Rating), -1);
            return new Result<ReviewDto> { IsSuccess = true, Message = "Review deleted successfully" };
        }
    }
}