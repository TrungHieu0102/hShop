using Core.Model;

namespace Application.Interfaces;

public interface IPaymentService
{
    Task<Result<string>> ProcessPaymentAsync(Guid userId);
}