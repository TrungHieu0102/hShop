using CloudinaryDotNet.Actions;
using Core.Entities;
using Core.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file, string folder);
        Task<DeletionResult> DeletePhotoAsync(string folder, string publicId);
        Task<Result<ProductImage>> UpdateProductImagesAsync(Guid productId, List<IFormFile> newImages);
        Task<Result<ReviewImage>> UpdateReviewImage(Guid reviewId, ICollection<IFormFile> reviewImages);
    }
}
