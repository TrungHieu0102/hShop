using Core.Model;

namespace Application.Interfaces;

public interface IPaypalService
{
    Task<Result<string>> CreatePaymentAsync(Guid orderId);
    Task<Result<string>> ExecutePaymentAsync(string paymentId, string payerId, Guid orderId);
}