using System.Globalization;
using Application.DTOs.CardDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CartService(IUnitOfWorkBase unitOfWork, ILogger<Cart> logger, IMapper mapper) : ICartService
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
                cartItem.UnitPrice += productInItem.Price * quantity;
                await unitOfWork.Carts.UpdateCartItemAsync(cartItem);
            }
            else
            {
                // Tạo mới cart item
                var newCartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = product.Price * quantity
                };
                await unitOfWork.Carts.AddCartItemAsync(newCartItem);
            }

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
            var cart = await unitOfWork.Carts.GetCartWithItemsAsync(userId);
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
                await unitOfWork.Carts.ClearCartAsync(cart.Id);
                await transaction.CommitAsync();
                return new Result<Cart>()
                {
                    IsSuccess = true,
                    Message = "Cart cleared successfully"
                };
            }

            var cartItem = cart.Items
                .FirstOrDefault(x => x.CartId == cart.Id && x.ProductId == productId);

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
            var cart = await unitOfWork.Carts.GetCartByUserId(userId, false);
            if (cart == null)
            {
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = "Cart not found"
                };
            }

            await unitOfWork.Carts.ClearCartAsync(cart.Id);
            await transaction.CommitAsync();
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
            var cart = await unitOfWork.Carts.GetCartByUserId(userId, true);
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
            var cart = await unitOfWork.Carts.GetCartByUserId(userId, true);
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