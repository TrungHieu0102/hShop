using Application.DTOs.TransactionDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Core.SeedWorks.Constants.Permissions;

namespace Application.Services
{
    public class TransactionService(IUnitOfWorkBase unitOfWork,
        IMapper mapper, ILogger<TransactionService> logger,
        ICacheService cacheService) : ITransactionService
    {


        public async Task<Result<TransactionDto>> CreateTransaction(CreateUpdateTransactionDto request)
        {
            try
            {
                var orderExists = await unitOfWork.Transactions.CheckOrderExists(request.OrderId);

                if (orderExists)
                {
                    throw new Exception("Order has been paymented");
                }
                var transaction = mapper.Map<PaymentTransaction>(request);
                transaction.TransactionId = Guid.NewGuid();
                var result = await unitOfWork.Transactions.AddTransactionAsync(transaction);
                if (!result)
                {
                    throw new Exception("Failed to create transaction");
                }
                await unitOfWork.CompleteAsync();
                return new Result<TransactionDto>
                {
                    IsSuccess = true,
                    Data = mapper.Map<TransactionDto>(transaction)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new Result<TransactionDto>
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
        public async Task<PagedResult<TransactionDto>> GetAllTransaction(string? searchByPaymentMethod, int page, int pageSize, bool isDescending)
        {
            try
            {
                var transactions = await unitOfWork.Transactions.GetAllAsync();
                if (!string.IsNullOrEmpty(searchByPaymentMethod))
                {
                    transactions = transactions.Where(p => p.TransactionReference.Contains(searchByPaymentMethod, StringComparison.OrdinalIgnoreCase));
                }
                transactions = isDescending ? transactions.OrderByDescending(p => p.PaymentDate) : transactions.OrderBy(p => p.PaymentDate);
                var totalRows = transactions.Count();
                var pagedTransactions = transactions.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new PagedResult<TransactionDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = mapper.Map<IEnumerable<TransactionDto>>(pagedTransactions),
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError(message: ex.Message);
                return new PagedResult<TransactionDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = 0,
                    Results = [],
                    IsSuccess = false
                };
            }
        }
        public async Task<Result<TransactionDto>> GetTransactionById(Guid id)
        {
            try
            {
                var cacheKey = $"Transaction_{id}";
                var cachedTransaction = await cacheService.GetCachedDataAsync<TransactionDto>(cacheKey);
                var transaction = (mapper.Map<PaymentTransaction>(cachedTransaction)
                    ?? await unitOfWork.Transactions.GetByIdAsync(id)) ?? null;
                if (transaction == null)
                {
                    throw new Exception("Transaction not found");
                }
                var transactionDto = mapper.Map<TransactionDto>(transaction);

                await cacheService.SetCachedDataAsync(cacheKey, transactionDto);
                return new Result<TransactionDto>
                {
                    IsSuccess = true,
                    Data = transactionDto
                };

            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new Result<TransactionDto>
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
        public async Task<Result<TransactionDto>> UpdateTransaction(Guid id, CreateUpdateTransactionDto request)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var cacheKey = $"Transaction_{id}";
                var transactionCache = await cacheService.GetCachedDataAsync<TransactionDto>(cacheKey);
                var model = mapper.Map<PaymentTransaction>(transactionCache) ?? await unitOfWork.Transactions.GetByIdAsync(id);
                if (model == null)
                {
                    throw new Exception("Transaction not found");
                }
                model = mapper.Map(request, model);
                unitOfWork.Transactions.Update(model);
                var result = await unitOfWork.CompleteAsync();
                if (result != 1)
                {
                    throw new Exception("Failed to update transaction");
                }
                await unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                await cacheService.RemoveCachedDataAsync(cacheKey);
                return new Result<TransactionDto>
                {
                    IsSuccess = true,
                    Data = mapper.Map<TransactionDto>(model)
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                logger.LogError(e.Message);
                return new Result<TransactionDto>
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
        public async Task<Result<TransactionDto>> DeleteTransaction(Guid id)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var cacheKey = $"Transaction_{id}";
                var transactionCache = await cacheService.GetCachedDataAsync<TransactionDto>(cacheKey);
                var model = mapper.Map<PaymentTransaction>(transactionCache) ?? await unitOfWork.Transactions.GetByIdAsync(id);
                if (model == null)
                {
                    throw new Exception("Transaction not found");
                }
                unitOfWork.Transactions.Delete(model);
                var result = await unitOfWork.CompleteAsync();
                if (result != 1)
                {
                    throw new Exception("Failed to delete transaction");
                }
                await cacheService.RemoveCachedDataAsync(cacheKey);
                await transaction.CommitAsync();
                return new Result<TransactionDto>
                {
                    IsSuccess = true,
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                logger.LogError(e.Message);
                return new Result<TransactionDto>
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
        public async Task<PagedResult<TransactionDto>> GetTransactionByUserId(Guid userId, int page, int pageSize, bool isDescending)
        {
            try
            {
                var transaction = await unitOfWork.Transactions.GetTransactionByUserID(userId);
                if (!transaction.Any())
                {
                    throw new Exception("Transaction not found");
                }
                transaction = isDescending ? transaction.OrderByDescending(p => p.PaymentDate) : transaction.OrderBy(p => p.PaymentDate);
                var totalRows = transaction.Count();
                var pageTransactions = transaction.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PagedResult<TransactionDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = mapper.Map<IEnumerable<TransactionDto>>(pageTransactions),
                    IsSuccess = true
                };

            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new PagedResult<TransactionDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = 0,
                    Results = [],
                    IsSuccess = false
                };
                throw;
            }
        }
        public async Task<decimal> GetTotalAmountByUserId(Guid userId)
        {
            var users = await unitOfWork.Transactions.GetTransactionByUserID(userId);
            var totalAmount = users.Sum(x => x.Amount);
            return totalAmount;
        }
        public async Task<PagedResult<TransactionDto>> GetTransactionsByDateRange(DateTime startDate, DateTime endDate, int page, int pageSize, bool isDescending)
        {
            try
            {
                var transactions = await unitOfWork.Transactions.GetAllAsync();
                transactions = transactions.Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate);

                if (!transactions.Any())
                {
                    throw new Exception("Transaction not found");
                }
                transactions = isDescending ? transactions.OrderByDescending(p => p.PaymentDate) : transactions.OrderBy(p => p.PaymentDate);
                var totalRows = transactions.Count();
                var pagedTransactions = transactions.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PagedResult<TransactionDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = totalRows,
                    Results = mapper.Map<IEnumerable<TransactionDto>>(pagedTransactions),
                    IsSuccess = true
                };
            }
            catch(Exception e)
            {
                logger.LogError(e.Message);
                return new PagedResult<TransactionDto>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = 0,
                    Results = [],
                    IsSuccess = false
                };
            }
        }
        public async Task<Dictionary<PaymentMethod, int>>GetTransactionStatistics()
        {
            var transactions = await unitOfWork.Transactions.GetAllAsync();
            var statistics = transactions
                .GroupBy(t => t.PaymentMethod)
                .Select(g => new { PaymentMethod = g.Key, Count = g.Count() })
                .ToDictionary(g => g.PaymentMethod, g => g.Count);
            return statistics;
        }
    }
}
