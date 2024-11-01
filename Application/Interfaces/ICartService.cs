using Application.DTOs.CardDto;
using Core.Entities;
using Core.Model;

namespace Application.Interfaces;

public interface ICartService
{
    Task<Result<Cart>> AddToCartAsync(Guid userId, Guid productId, int quantity);
    Task<Result<Cart>> RemoveFromCartAsync(Guid userId, Guid productId);
    Task<Result<Cart>> ClearCartAsync(Guid userId);
    Task<Result<CartDto>> GetCartDetailsAsync(Guid userId);
    Task<Result<Cart>> UpdateCartQuantityAsync(Guid userId, Guid productId, int quantity);

}