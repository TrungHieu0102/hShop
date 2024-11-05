using Application.DTOs.TransactionDto;
using Core.Entities;
using Core.Model;

namespace Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Result<TransactionDto>> CreateTransaction(CreateUpdateTransactionDto request);
        Task<PagedResult<TransactionDto>> GetAllTransaction(string? searchByPaymentMethod, int page, int pageSize, bool isDescending);
        Task<Result<TransactionDto>> GetTransactionById(Guid id);
        Task<Result<TransactionDto>> UpdateTransaction(Guid id, CreateUpdateTransactionDto request);
        Task<Result<TransactionDto>> DeleteTransaction(Guid id);
        Task<PagedResult<TransactionDto>> GetTransactionByUserId(Guid userId, int page, int pageSize, bool isDescending);
        Task<decimal> GetTotalAmountByUserId(Guid userId);
        Task<PagedResult<TransactionDto>> GetTransactionsByDateRange(DateTime startDate, DateTime endDate, int page, int pageSize, bool isDescending);
        Task<Dictionary<PaymentMethod, int>> GetTransactionStatistics();
    }
}
