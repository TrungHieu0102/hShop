using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class PhotoService(Cloudinary cloudinary, IUnitOfWorkBase unitOfWork) : IPhotoService
    {
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file, string folder)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = folder 

                };
                uploadResult = await cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string folder,string publicId)
        {
            var deleteParams = new DeletionParams($"{folder}/{publicId}");
            var result = await cloudinary.DestroyAsync(deleteParams);
            return result;
        }
        public async Task<Result<ProductImage>> UpdateProductImagesAsync(Guid productId, List<IFormFile> newImages)
        {
            var existingImages = await unitOfWork.Images.GetImagesByProductIdAsync(productId);
            if (existingImages.Count > 0)
            {
                foreach (var image in existingImages)
                {
                    await DeletePhotoAsync("product", image.ImageUrl);
                }

                await unitOfWork.Images.DeleteImagesByProductIdAsync(productId);
            }

            if (newImages != null && newImages.Count != 0)
            {
                foreach (var image in newImages)
                {
                    var uploadResult = await AddPhotoAsync(image, "product");
                    if (uploadResult.Error == null)
                    {
                        var productImage = new ProductImage
                        {
                            Id = Guid.NewGuid(),
                            ProductId = productId,
                            ImageUrl = uploadResult.SecureUrl.ToString(),
                            IsPrimary = existingImages.Count == 0,  
                            ImageType = "Product"
                        };

                        unitOfWork.Images.Add(productImage);
                    }
                }
            }
            await unitOfWork.CompleteAsync();
            return new Result<ProductImage> { IsSuccess = true };
        }
        public async Task<Result<ReviewImage>> UpdateReviewImage(Guid reviewId, ICollection<IFormFile> reviewImages)
        {
            var existingImages = await unitOfWork.ReviewImages.GetImagesByReviewIdAsync(reviewId);
            if (existingImages.Count > 0)
            {
                foreach (var image in existingImages)
                {
                    await DeletePhotoAsync("product", image.ImageUrl);
                }

                await unitOfWork.ReviewImages.DeleteImagesByReviewIdAsync(reviewId);
            }

            if (reviewImages != null && reviewImages.Count != 0)
            {
                foreach (var image in reviewImages)
                {
                    var uploadResult = await AddPhotoAsync(image, "review");
                    if (uploadResult.Error == null)
                    {
                        var reviewImg = new ReviewImage
                        {
                            Id = Guid.NewGuid(),
                            ReviewId = reviewId,
                            ImageUrl = uploadResult.SecureUrl.ToString(),
                           
                        };
                        unitOfWork.ReviewImages.Add(reviewImg);
                    }
                }
            }
            await unitOfWork.CompleteAsync();
            return new Result<ReviewImage> { IsSuccess = true };
        }
    }
}
