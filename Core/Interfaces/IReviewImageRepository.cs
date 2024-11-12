using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReviewImageRepository :IRepositoryBase<ReviewImage, Guid> 
    {
        Task<List<ReviewImage>> GetImagesByReviewIdAsync(Guid reviewId);
        Task DeleteImagesByReviewIdAsync(Guid reviewId);

    }
}
