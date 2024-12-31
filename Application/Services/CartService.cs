using System.Globalization;
using Application.DTOs.CardDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CartService(IUnitOfWorkBase unitOfWork, ILogger<Cart> logger, IMapper mapper, ICacheService cacheService)
    : ICartService
{
    public async Task<Result<Cart>> AddToCartAsync(Guid userId, Guid productId, int quantity)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId);
            if (product == null || product.Quantity < quantity)
            {
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = "Sản phẩm không tồn tại hoặc không đủ số lượng"
                };
            }

            var cart = await unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await unitOfWork.Carts.CreateCartAsync(cart);
            }

            var cartItem = await unitOfWork.Carts.GetCartItemAsync(cart.Id, productId);
            if (cartItem != null)
            {
                var productInItem = await unitOfWork.Products.GetByIdAsync(productId);
                cartItem.Quantity += quantity;
                await unitOfWork.Carts.UpdateCartItemAsync(cartItem);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                await unitOfWork.Carts.AddCartItemAsync(newCartItem);
            }

            var cacheKey = $"Cart-{userId}";
            await cacheService.RemoveCachedDataAsync(cacheKey);
            await transaction.CommitAsync();
            return new Result<Cart>()
            {
                IsSuccess = true,
                Message = "Thêm sản phẩm thành công"
            };
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await transaction.RollbackAsync();
            return new Result<Cart>()
            {
                IsSuccess = false,
                Message = e.Message,
            };
        }
    }
    public async Task<Result<Cart>> RemoveFromCartAsync(Guid userId, Guid productId)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var cacheKey = $"Cart-{userId}";
            var cartInCache = await cacheService.GetCachedDataAsync<CartDto>(cacheKey);
            var cart = mapper.Map<Cart>(cartInCache) ?? await unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = "Cart not found"
                };
            }

            if (cart.Items.Count == 1 && cart.Items.Any(x => x.ProductId == productId))
            {
                await unitOfWork.Carts.ClearCartAsync(userId);
                await unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                await cacheService.RemoveCachedDataAsync(cacheKey);
                return new Result<Cart>()
                {
                    IsSuccess = true,
                    Message = "Cart cleared successfully"
                };
            }

            var cartItem = cart.Items
                .FirstOrDefault(x => x.ProductId == productId);

            if (cartItem == null)
            {
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = "Cart item not found"
                };
            }

            await unitOfWork.Carts.RemoveCartItemAsync(cartItem);
            await transaction.CommitAsync();
            await cacheService.RemoveCachedDataAsync(cacheKey);
            return new Result<Cart>()
            {
                IsSuccess = true,
                Message = "Item removed successfully"
            };
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await transaction.RollbackAsync();
            return new Result<Cart>()
            {

                IsSuccess = false,
                Message = e.Message
            };
        }
    }

    public async Task<Result<Cart>> ClearCartAsync(Guid userId)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var cacheKey = $"Cart-{userId}";
            var cartInCache = await cacheService.GetCachedDataAsync<CartDto>(cacheKey);
            var cart = mapper.Map<Cart>(cartInCache) ?? await unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = "Cart not found"
                };
            }

            await unitOfWork.Carts.ClearCartAsync(userId);
            await unitOfWork.CompleteAsync();
            await transaction.CommitAsync();
            await cacheService.RemoveCachedDataAsync(cacheKey);
            return new Result<Cart>()
            {
                IsSuccess = true,
                Message = "Clear cart successfully"
            };
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await transaction.RollbackAsync();
            return new Result<Cart>()
            {
                IsSuccess = false,
                Message = e.Message
            };
        }
    }

    public async Task<Result<CartDto>> GetCartDetailsAsync(Guid userId)
    {
        try
        {
            var cacheKey = $"Cart-{userId}";
            var cartInCache = await cacheService.GetCachedDataAsync<CartDto>(cacheKey);
            var cart = mapper.Map<Cart>(cartInCache) ?? await unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                return new Result<CartDto>()
                {
                    IsSuccess = false,
                    Message = "Cart not found"
                };
            }

            var cartDto = new CartDto()
            {
                Id = cart.Id,
                UserId = userId,
                Items = cart.Items.Select(ci => new CartItemsDto()
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.ProductName,
                    CartId = ci.CartId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList()
            };
            await cacheService.SetCachedDataAsync(cacheKey, cartDto);
            return new Result<CartDto>()
            {
                IsSuccess = true,
                Data = cartDto
            };
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred while get the cart: {ErrorMessage}", e.Message);
            return new Result<CartDto>()
            {
                IsSuccess = false,
                Message = e.Message
            };
        }
    }

    public async Task<Result<Cart>> UpdateCartQuantityAsync(Guid userId, Guid productId, int quantity)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var cacheKey = $"Cart-{userId}";
            var cartInCache = await cacheService.GetCachedDataAsync<CartDto>(cacheKey);
            var cart = mapper.Map<Cart>(cartInCache) ?? await unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }

            var cartItem = await unitOfWork.Carts.GetCartItemAsync(cart.Id, productId);
            if (cartItem == null)
            {
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = "Cart item not found"
                };
            }
            cartItem.Quantity = quantity;
            await unitOfWork.Carts.UpdateCartItemAsync(cartItem);
            await cacheService.RemoveCachedDataAsync(cacheKey);
            await unitOfWork.CompleteAsync();
            await transaction.CommitAsync();
            return new Result<Cart>()
            {
                IsSuccess = true,
            };
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred while get the cart: {ErrorMessage}", e.Message);
            return new Result<Cart>()
            {
                IsSuccess = false,
                Message = e.Message
            };
        }
    }
}