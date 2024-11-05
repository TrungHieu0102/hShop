using Application.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrl(HttpContext context, Guid orderId);
        Task<VnPaymentResponseModel> PaymentExecute(IQueryCollection collections);
    }
}
