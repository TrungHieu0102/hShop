using Application.DTOs.WishlistDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;

namespace Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly IMapper _mapper;
        public WishlistService(IUnitOfWorkBase unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<WishlistDto>> AddWishlist(CreateUpdateWishlistDto wishlist)
        {
            try
            {
                var checkExists = await _unitOfWork.Wishlists.CheckExists(wishlist.UserId, wishlist.ProductId);
                if (checkExists)
                {
                    throw new Exception("Product already exists in wishlist");
                }
                var wishlistEntity = _mapper.Map<Wishlist>(wishlist);
                _unitOfWork.Wishlists.Add(wishlistEntity);
                await _unitOfWork.CompleteAsync();
                return new Result<WishlistDto>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<WishlistDto>(wishlistEntity)
                };
            }
            catch (Exception ex)
            {
                return new Result<WishlistDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ResultBase> DeleteWishlist(Guid userId , Guid productId)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var wishlist = await _unitOfWork.Wishlists.GetWishlist(userId, productId)
                                ?? throw new Exception("Product not found in wishlist");
                _unitOfWork.Wishlists.Delete(wishlist);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return new ResultBase
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResultBase
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<PagedResult<WishlistDto>> GetWishlistByUserid(Guid userId, int page, int pageSize)
        {
            var wishlists = await _unitOfWork.Wishlists.GetWishListByUserId(userId);
            var pagedWishlists = wishlists.Skip((page - 1) * pageSize).Take(pageSize);
            var result = _mapper.Map<List<WishlistDto>>(pagedWishlists);
            return new PagedResult<WishlistDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = pagedWishlists.Count(),
                Results = result,
                IsSuccess = true
            };
        }

    }
}
