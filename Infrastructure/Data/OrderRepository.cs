

using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
#nullable  disable
namespace Infrastructure.Data
{
    public class OrderRepository(HshopContext context) : RepositoryBase<Order, Guid>(context), IOrderRepository
    {
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails) 
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrderByUserId(Guid userId)
        {
            var orders = await _context.Orders
                                    .Where(p => p.UserId == userId)
                                    .Include(p => p.OrderDetails)
                                    .ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<Order>> GetAllOrderAsync()
        {
           return await _context.Orders.Include(p=>p.OrderDetails).ToListAsync();
        }
    }
}
