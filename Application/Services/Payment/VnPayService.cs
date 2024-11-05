using Application.Common.Shared;
using Application.DTOs.OrdersDto;
using Application.DTOs.TransactionDto;
using Application.Extensions;
using Application.Interfaces;
using Application.Model;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Services.Payment
{
    public class VnPayService(IOptions<VnPaySettings> vnPaySettings, 
        IUnitOfWorkBase unitOfWork, 
        ITransactionService transaction, 
        ICacheService cacheService,
        IOrderService orderService)
        : IVnPayService
    {
        private readonly VnPaySettings _vnPaySettings = vnPaySettings.Value;

        public async Task<string> CreatePaymentUrl(HttpContext context, Guid orderId)
        {
            try
            {
                var cacheKey = $"Order-{orderId}";
                var orderExists = await unitOfWork.Transactions.CheckOrderExists(orderId);
                if (orderExists)
                {
                    throw new Exception("Order has been paymented");
                }
                var orderInCache = await cacheService.GetCachedDataAsync<OrderDto>(cacheKey);
                var order = (orderInCache ?? (await orderService.GetOrderAsync(orderId)).Data) ?? null;
                var tick = DateTime.Now.Ticks.ToString();
                if (order == null || order.PaymentStatus != PaymentStatus.Pending.ToString() 
                    && order.PaymentStatus != PaymentStatus.Failed.ToString())
                {
                    throw new Exception("Order is not eligible for payment !");
                }
                var vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", _vnPaySettings.Version);
                vnpay.AddRequestData("vnp_Command", _vnPaySettings.Command);
                vnpay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
                vnpay.AddRequestData("vnp_Amount", (((Int64)order.TotalAmount * 25000) * 100).ToString());
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", _vnPaySettings.CurrCode);
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
                vnpay.AddRequestData("vnp_Locale", _vnPaySettings.Locale);
                vnpay.AddRequestData("vnp_OrderInfo", "OtherInfo = " + orderId);
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", _vnPaySettings.PaymentBackReturnUrl);
                vnpay.AddRequestData("vnp_TxnRef", tick);
                var paymentUrl = vnpay.CreateRequestUrl(_vnPaySettings.BaseUrl, _vnPaySettings.HashSecret);
                return paymentUrl;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public async Task<VnPaymentResponseModel> PaymentExecute(IQueryCollection collections)
        {
            try
            {
                var vnpay = new VnPayLibrary();
                foreach (var (key, value) in collections)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }
                var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
                var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash!, _vnPaySettings.HashSecret);
                if (!checkSignature)
                {
                    throw new Exception("Invalid signature");
                }
                if (vnp_ResponseCode != "00")
                {
                    throw new Exception($"Payment failed. Response code {vnp_ResponseCode}");
                }
                var orderId = vnp_OrderInfo.Substring(vnp_OrderInfo.IndexOf('=') + 1).Trim();
                var cacheKey = $"Order-{orderId}";

                var order = await unitOfWork.Orders.GetOrderById(Guid.Parse(orderId));
                if (order == null)
                {
                    throw new Exception("Order not found");
                }
                var createTransactionRequest = new CreateUpdateTransactionDto()
                {
                    OrderId = Guid.Parse(orderId),
                    UserId = order.UserId,
                    PaymentDate = DateTime.Now,
                    Amount = order.TotalAmount,
                    PaymentMethod = "Vnpay",
                    TransactionReference = $"VNPAY_{orderId}",
                    Status = "Success",
                    Currency = "USD",
                    Notes = "Vnpay Payment"
                };
                var transactionResult = await transaction.CreateTransaction(createTransactionRequest);
                if (!transactionResult.IsSuccess)
                {
                    throw new Exception(transactionResult.Message);
                }
                order.PaymentStatus = PaymentStatus.Completed;
                order.PaymentMethod = PaymentMethod.VnPay;
                unitOfWork.Orders.Entry(order).Property(o => o.PaymentStatus).IsModified = true;
                unitOfWork.Orders.Entry(order).Property(o => o.PaymentMethod).IsModified = true;
                await unitOfWork.CompleteAsync();
                await cacheService.RemoveCachedDataAsync(cacheKey);

                return new VnPaymentResponseModel
                {
                    Success = true,
                    PaymentMethod = "VnPay",
                    OrderDescription = vnp_OrderInfo,
                    OrderId = vnp_orderId.ToString(),
                    TransactionId = vnp_TransactionId.ToString(),
                    Token = vnp_SecureHash!,
                    VnPayResponseCode = vnp_ResponseCode
                };
            }
            catch (Exception ex)
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
