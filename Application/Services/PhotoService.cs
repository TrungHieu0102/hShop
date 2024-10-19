using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IUnitOfWorkBase _unitOfWork;
        public PhotoService(Cloudinary cloudinary,IUnitOfWorkBase unitOfWork)
        {
            _cloudinary = cloudinary;
            _unitOfWork = unitOfWork;
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file, string folder)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                        Folder = folder 

                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string folder,string publicId)
        {
            var deleteParams = new DeletionParams($"{folder}/{publicId}");
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result;
        }
        public async Task<Result<ProductImage>> UpdateProductImagesAsync(Guid productId, List<IFormFile> newImages)
        {
            var existingImages = await _unitOfWork.Images.GetImagesByProductIdAsync(productId);
            if (existingImages.Count > 0)
            {
                foreach (var image in existingImages)
                {
                    await DeletePhotoAsync("product", image.ImageUrl);
                }

                await _unitOfWork.Images.DeleteImagesByProductIdAsync(productId);
            }

            if (newImages != null && newImages.Any())
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

                        _unitOfWork.Images.Add(productImage);
                    }
                }
            }
            await _unitOfWork.CompleteAsync();
            return new Result<ProductImage> { IsSuccess = true };
        }
    }
}
