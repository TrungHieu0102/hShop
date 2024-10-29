

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
    }
}
