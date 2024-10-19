using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IImageRepository : IRepositoryBase<ProductImage, Guid>
    {
        Task<List<ProductImage>> GetImagesByProductIdAsync(Guid productId);
        Task DeleteImagesByProductIdAsync(Guid productId);
    }
}
