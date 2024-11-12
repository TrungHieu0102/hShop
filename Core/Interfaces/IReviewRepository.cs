using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReviewRepository :IRepositoryBase<Review, Guid>
    {
        Task<IEnumerable<Review>> GetReviewByProductId(Guid id);
        Task<bool> HasReview(Guid productId, Guid userId);
        Task<List<Review>> GetReviewByUserId(Guid userId);
    }
}
