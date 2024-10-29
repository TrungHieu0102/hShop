using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    
    public class CartRepository : RepositoryBase<Cart, Guid>, ICartRepository
    {

        public CartRepository(HshopContext context) : base(context)
        {
            
        }

        public async Task<Cart?> GetCart(Guid userId)
        {
            return await _context.Carts.FirstOrDefaultAsync(u=>u.UserId == userId);
        }
        public async Task CreateCartAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();  
        }
        public async Task DeleteCartAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
        public async Task AddCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveCartItemAsync(CartItem cartItem)
        {
           
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid cartId)
        {
            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == cartId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<Cart?> GetCartWithItemsAsync(Guid userId)
        {
            return await _context.Carts
                .Include(c => c.Items)  
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId)
        {
            var cartItem =  await _context.CartItems.FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);
            if (cartItem == null)
            {
                return null;
            }

            return cartItem;
        }
    }
}