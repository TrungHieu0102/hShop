using Core.Entities;

namespace Core.Interfaces;

public interface ICartRepository
{
    Task AddCartItemAsync(CartItem cartItem);
    Task RemoveCartItemAsync(CartItem cartItem);
    Task ClearCartAsync(Guid userId);
    Task CreateCartAsync(Cart cart);
    Task DeleteCartAsync(Cart cart);
    Task<Cart> GetCart(Guid userId);
    Task UpdateCartItemAsync(CartItem cartItem);
    Task<Cart> GetCartWithItemsAsync(Guid userId);
    Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId);
}