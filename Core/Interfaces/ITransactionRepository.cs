using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITransactionRepository :IRepositoryBase<PaymentTransaction,Guid>
    {
        Task<bool> CheckOrderExists(Guid orderId);
        Task<bool> AddTransactionAsync(PaymentTransaction transaction);
        Task<IEnumerable<PaymentTransaction>> GetTransactionByUserID(Guid userId);

    }
}
