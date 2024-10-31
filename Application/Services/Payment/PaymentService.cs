using Application.Interfaces;
using Core.ConfigOptions;
using Core.Entities;
using Core.Model;
using Microsoft.Extensions.Logging;
using PayPal;
using PayPal.Api;

namespace Application.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly PaypalConfiguration _paypalConfiguration;
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(PaypalConfiguration paypalConfiguration, IOrderService orderService, ILogger<PaymentService> logger)
        {
            _paypalConfiguration = paypalConfiguration;
            _orderService = orderService;
            _logger = logger;
        }
        public async Task<Result<string>> CreatePaymentAsync(Guid orderId)
        {
            var orderResult = await _orderService.GetOrderAsync(orderId);
            var orderStatus = orderResult.Data.PaymentStatus;
            if (!orderResult.IsSuccess || (orderStatus != PaymentStatus.Pending.ToString() && orderStatus != PaymentStatus.Failed.ToString()))
            {
                return new Result<string>
                {
                    IsSuccess = false,
                    Message = "Order is not eligible for payment."
                };
            }
            var apiContext = _paypalConfiguration.GetAPIContext();
            var itemList = new ItemList() { items = new List<Item>() };
            decimal subtotal = 0;

            foreach (var detail in orderResult.Data.OrderDetails)
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
                    return_url = $"http://localhost:5000/api/payment/execute-payment?orderId={orderId}"
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
                _logger.LogError($"PayPal Payment Exception: {ex.Message}");
                _logger.LogError($"PayPal Payment Exception Details: {ex.Response}");
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
                var apiContext = _paypalConfiguration.GetAPIContext();
                var paymentExecution = new PaymentExecution() { payer_id = payerId };
                var payment = new PayPal.Api.Payment() { id = paymentId };

                // Thực hiện thanh toán
                var executedPayment = payment.Execute(apiContext, paymentExecution);

                // Kiểm tra trạng thái thanh toán
                if (executedPayment.state.ToLower() != "approved")
                {
                    return new Result<string>
                    {
                        IsSuccess = false,
                        Message = "Payment not approved."
                    };
                }

                // Cập nhật trạng thái thanh toán
                var updatePaymentResult = await _orderService.UpdatePaymentStatusAsync(orderId, PaymentStatus.Completed);
                if (!updatePaymentResult.IsSuccess)
                {
                    return new Result<string>
                    {
                        IsSuccess = false,
                        Message = updatePaymentResult.Message
                    };
                }

                return new Result<string>
                {
                    IsSuccess = true,
                    Message = "Payment executed successfully."
                };
            }
            catch (PayPal.PaymentsException ex)
            {
                _logger.LogError($"PayPal Payment Execution Error: {ex.Message}");
                _logger.LogError($"Details: {ex.Response}");

                return new Result<string>
                {
                    IsSuccess = false,
                    Message = $"Error executing payment: {ex.Message}"
                };
            }
        }

    }
}
