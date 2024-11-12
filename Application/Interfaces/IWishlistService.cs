using Application.DTOs.WishlistDto;
using Core.Entities;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IWishlistService
    {
        Task<Result<WishlistDto>> AddWishlist(CreateUpdateWishlistDto wishlist);
        Task<ResultBase> DeleteWishlist(Guid userId, Guid productId);
        Task<PagedResult<WishlistDto>> GetWishlistByUserid(Guid userId, int page, int pageSize);
    }
}
