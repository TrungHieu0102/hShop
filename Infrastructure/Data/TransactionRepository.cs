using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
#nullable disable
namespace Infrastructure.Data
{
    public class TransactionRepository(HshopContext context) : RepositoryBase<PaymentTransaction, Guid>(context), ITransactionRepository
    {
        public async Task<bool> CheckOrderExists(Guid orderId)
        {
            return await _context.PaymentTransactions.AnyAsync(t => t.OrderId == orderId);
        }
        public async Task<bool> AddTransactionAsync(PaymentTransaction transaction)
        {
            var result = await _context.PaymentTransactions.AddAsync(transaction);
            if (result.State == EntityState.Added)
            {
                return true;
            }
            return false;
        }
        public async Task<IEnumerable<PaymentTransaction>> GetTransactionByUserID(Guid userId)
        {
            var transactions = await _context.PaymentTransactions.Where(t => t.UserId == userId).ToListAsync();
            return transactions;
        }
        public async Task<IEnumerable<PaymentTransaction>> GetSalesInDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.PaymentTransactions.Where(t => t.PaymentDate >= startDate && t.PaymentDate <= endDate).ToListAsync();
        }

    }
}
