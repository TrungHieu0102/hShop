using Application.DTOs.OrdersDto;
using Application.DTOs.TransactionDto;
using Application.Interfaces;
using AutoMapper;
using Core.ConfigOptions;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using PayPal;
using PayPal.Api;
using System.Net;

namespace Application.Services.Payment
{
    public class PaypalService(
        PaypalConfiguration paypalConfiguration,
        IUnitOfWorkBase unitOfWork,
        ICacheService cacheService,
        IOrderService orderService,
        ITransactionService transactionService,
        ILogger<PaypalService> logger)
        : IPaypalService
    {
        public async Task<Result<string>> CreatePaymentAsync(Guid orderId)
        {
            var cacheKey = $"Order-{orderId}";
            var orderExists = await unitOfWork.Transactions.CheckOrderExists(orderId);
            if(orderExists)
            {
                return new Result<string>
                {
                    IsSuccess = false,
                    Message = "Transaction Services : Order has been paymented"
                };
            }
            var orderInCache = await cacheService.GetCachedDataAsync<OrderDto>(cacheKey);
            var order = (orderInCache ?? (await orderService.GetOrderAsync(orderId)).Data) ?? null;
            if (order == null || order.PaymentStatus != PaymentStatus.Pending.ToString() && order.PaymentStatus != PaymentStatus.Failed.ToString())
            {
                return new Result<string>
                {
                    IsSuccess = false,
                    Message = "Order is not eligible for payment."
                };
            }
            var apiContext = paypalConfiguration.GetAPIContext();
            var itemList = new ItemList() { items = new List<Item>() };
            decimal subtotal = 0;
            foreach (var detail in order.OrderDetails)
            {
                var itemPrice = detail.UnitPrice;
                var itemQuantity = detail.Quantity;

                itemList.items.Add(new Item()
                {
                    name = detail.ProductName,
                    currency = "USD",
                    price = itemPrice.ToString("F2"),
                    quantity = itemQuantity.ToString(),
                    sku = detail.ProductId.ToString()
                });
                subtotal += itemPrice * itemQuantity;
            }
            var details = new Details()
            {
                tax = "0.00",
                shipping = "0.00",
                subtotal = subtotal.ToString("F2")
            };
            var amount = new Amount()
            {
                currency = "USD",
                total = (subtotal + 0.00m).ToString("F2"),
                details = details
            };
            var uniqueInvoiceNumber = $"{orderId}-{Guid.NewGuid()}";
            var transactionList = new List<Transaction>
            {
                new Transaction()
                {
                    description = $"Order ID: {orderId}",
                    invoice_number = uniqueInvoiceNumber,
                    amount = amount,
                    item_list = itemList
                }
            };

            var payment = new PayPal.Api.Payment()
            {
                intent = "sale",
                payer = new Payer() { payment_method = "paypal" },
                transactions = transactionList,
                redirect_urls = new RedirectUrls()
                {
                    cancel_url = "http://localhost:5000/api/payment/cancel",
                    return_url = $"http://localhost:5000/api/payment/paypal/execute-payment?orderId={orderId}"
                }
            };
            try
            {
                var createdPayment = payment.Create(apiContext);
                return new Result<string>
                {
                    IsSuccess = true,
                    Data = createdPayment.links.FirstOrDefault(x => x.rel.ToLower() == "approval_url")?.href
                };
            }
            catch (PaymentsException ex)
            {
                logger.LogError($"PayPal Payment Exception: {ex.Message}");
                logger.LogError($"PayPal Payment Exception Details: {ex.Response}");
                return new Result<string>
                {
                    IsSuccess = false,
                    Message = "An error occurred while creating the payment."
                };
            }
        }
        public async Task<Result<string>> ExecutePaymentAsync(string paymentId, string payerId, Guid orderId)
        {
            try
            {
                var cacheKey = $"Order-{orderId}";
                var apiContext = paypalConfiguration.GetAPIContext();
                var paymentExecution = new PaymentExecution() { payer_id = payerId };
                var payment = new PayPal.Api.Payment() { id = paymentId };
                var executedPayment = payment.Execute(apiContext, paymentExecution);
                if (executedPayment.state.ToLower() != "approved")
                {
                    throw new Exception("Payment not approved.");
                }
                var order = await unitOfWork.Orders.GetByIdAsync(orderId)
                            ?? throw new Exception("Order not fount");
                var createTransactionRequest = new CreateUpdateTransactionDto()
                {
                    OrderId = orderId,
                    UserId = order.UserId,
                    PaymentDate = DateTime.Now,
                    Amount = order.TotalAmount,
                    PaymentMethod = "Paypal",
                    TransactionReference = $"PAYPAL_{orderId}",
                    Status = "Success",
                    Currency = "USD",
                    Notes = "PayPal Payment"
                };
                var transactionResult = await transactionService.CreateTransaction(createTransactionRequest);
                if (!transactionResult.IsSuccess)
                {
                    throw new Exception(transactionResult.Message);
                }
                order.PaymentStatus = PaymentStatus.Completed;
                order.PaymentMethod = PaymentMethod.Paypal;
                unitOfWork.Orders.Entry(order).Property(o => o.PaymentStatus).IsModified = true;
                unitOfWork.Orders.Entry(order).Property(o => o.PaymentMethod).IsModified = true;
                await unitOfWork.CompleteAsync();
                await cacheService.RemoveCachedDataAsync(cacheKey);
                return new Result<string>
                {
                    IsSuccess = true,
                    Message = "Payment executed successfully."
                };
            }
            catch (PaymentsException ex)
            {
                logger.LogError($"PayPal Payment Execution Error: {ex.Message}");
                logger.LogError($"Details: {ex.Response}");

                return new Result<string>
                {
                    IsSuccess = false,
                    Message = $"Error executing payment: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while executing payment: {ex.Message}");
                return new Result<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

    }
}
