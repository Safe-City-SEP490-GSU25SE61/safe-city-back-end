using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOS _payOS;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IPayosTransactionRepository _payosTransactionRepo;
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IPackageRepository _packageRepo;
        private readonly IConfiguration _configuration;
        public PaymentService(
            IConfiguration configuration,
            IPaymentRepository paymentRepo,
            IPayosTransactionRepository payosTransactionRepo,
            ISubscriptionRepository subscriptionRepo,
            IPackageRepository packageRepo)
        {
            _payOS = new PayOS(
                configuration["Environment:PAYOS_CLIENT_ID"]
                    ?? throw new Exception("Missing PAYOS_CLIENT_ID"),
                configuration["Environment:PAYOS_API_KEY"]
                    ?? throw new Exception("Missing PAYOS_API_KEY"),
                configuration["Environment:PAYOS_CHECKSUM_KEY"]
                    ?? throw new Exception("Missing PAYOS_CHECKSUM_KEY")
            );

            _paymentRepo = paymentRepo;
            _payosTransactionRepo = payosTransactionRepo;
            _subscriptionRepo = subscriptionRepo;
            _packageRepo = packageRepo;
            _configuration = configuration;
        }

        public async Task<CreatePaymentResult> CreatePaymentAsync(Guid userId, int packageId, string returnUrl, string cancelUrl)
        {
            var package = await _packageRepo.GetByIdAsync(packageId);
            if (package == null || !package.IsActive)
                throw new Exception("Invalid package");

            long orderCode = long.Parse(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

            var subscription = new Subscription
            {
                Id = 0,
                UserId = userId,
                PackageId = packageId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(package.DurationDays),
                IsActive = false
            };

            await _subscriptionRepo.AddAsync(subscription);

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = subscription.Id,
                Amount = package.Price,
                PaymentMethod = "PayOs",
                Status = "Pending",
                TransactionCode = orderCode.ToString(),
                PaidAt = null
            };

            await _paymentRepo.AddAsync(payment);

            var transaction = new PayosTransaction
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                OrderCode = orderCode.ToString(),
                QrCodeUrl = "",
                DeeplinkUrl = "",
                ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var item = new ItemData(package.Name, 1, (int)package.Price);
            var paymentData = new PaymentData(orderCode, (int)package.Price, $"{package.Name}", new List<ItemData> { item }, cancelUrl, returnUrl);
            var result = await _payOS.createPaymentLink(paymentData);

            transaction.QrCodeUrl = result.qrCode;
            transaction.DeeplinkUrl = result.checkoutUrl;

            await _payosTransactionRepo.AddAsync(transaction);

            return result;
        }

        public async Task HandleWebhookAsync(WebhookType webhookBody)
        {
            //string checksumKey = _configuration["Environment:PAYOS_CHECKSUM_KEY"]
            //        ?? throw new Exception("Missing PAYOS_CHECKSUM_KEY");
            string checksumKey = "2ea763b4ccb883a3247711b3d5e977ffb4d454b94986ec664840a0160f524031";
            string receivedSignature = webhookBody.signature;
            string rawData = BuildSignatureDataString(webhookBody.data);
            string computedSignature = GenerateHmacSha256Signature(rawData, checksumKey);

            if (receivedSignature != computedSignature)
                throw new Exception("Invalid signature");

            if (webhookBody.data.description == "VQRIO123")
            {
                Console.WriteLine("Received test webhook from PayOS, skipping real logic.");
                return;
            }

            var payment = await _paymentRepo.GetByOrderCodeAsync(webhookBody.data.orderCode.ToString());
            if (payment == null)
                throw new Exception("Order not found");

            if (webhookBody.data.code == "00")
            {
                payment.Status = "Paid";
                payment.PaidAt = DateTime.UtcNow;
                if (payment.Subscription != null)
                {
                    payment.Subscription.IsActive = true;
                    await _subscriptionRepo.UpdateAsync(payment.Subscription);
                }

                if (payment.PayosTransaction != null)
                {
                    payment.PayosTransaction.Status = "COMPLETED";
                    payment.PayosTransaction.WebhookReceivedAt = DateTime.UtcNow;
                    payment.PayosTransaction.UpdatedAt = DateTime.UtcNow;
                    await _payosTransactionRepo.UpdateAsync(payment.PayosTransaction);
                }

                await _paymentRepo.UpdateAsync(payment);
            }
        }


        private string BuildSignatureDataString(object data)
        {
            var keyValues = new SortedDictionary<string, string>();

            foreach (PropertyInfo prop in data.GetType().GetProperties())
            {
                var value = prop.GetValue(data)?.ToString() ?? "";
                keyValues[prop.Name] = value;
            }

            return string.Join("&", keyValues.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }

        private string GenerateHmacSha256Signature(string data, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }



        public async Task<PaymentStatusResponse?> GetPaymentStatusAsync(string orderCode)
        {
            var payment = await _paymentRepo.GetByOrderCodeAsync(orderCode);
            if (payment == null || payment.Status != "Paid")
                return null;

            return new PaymentStatusResponse
            {
                PackageName = payment.Subscription.Package.Name,
                Amount = payment.Amount,
                PaidAt = payment.PaidAt?.ToString("yyyy-MM-dd HH:mm:ss"),
                OrderCode = orderCode,
                Status = payment.Status
            };
        }

        public async Task ConfirmWebhookAsync(string webhookUrl)
        {
            await _payOS.confirmWebhook(webhookUrl);
        }

        public async Task<IEnumerable<PaymentHistoryResponseModel>> GetUserPaymentHistoryAsync(Guid userId)
        {
            var payments = await _paymentRepo.GetByUserIdAsync(userId);

            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            return payments.Select(payment => new PaymentHistoryResponseModel
            {
                OrderCode = payment.TransactionCode,
                Amount = payment.Amount,
                Quantity = 1,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaidAt = payment.PaidAt.HasValue
                    ? TimeZoneInfo.ConvertTimeFromUtc(payment.PaidAt.Value, vietnamTimeZone).ToString("yyyy-MM-dd HH:mm:ss")
                    : "Pending",
                PackageName = payment.Subscription?.Package?.Name ?? "N/A"
            });
        }
        public async Task<IEnumerable<AdminPaymentHistoryResponseModel>> GetAllPaymentHistoryForAdminAsync()
        {
            var payments = await _paymentRepo.GetAllAsync(); 

            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            return payments.Select(payment => new AdminPaymentHistoryResponseModel
            {
                OrderCode = payment.TransactionCode,
                Amount = payment.Amount,
                Quantity = 1,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaidAt = payment.PaidAt.HasValue
                    ? TimeZoneInfo.ConvertTimeFromUtc(payment.PaidAt.Value, vietnamTimeZone).ToString("yyyy-MM-dd HH:mm:ss")
                    : "Pending",
                PackageName = payment.Subscription?.Package?.Name ?? "N/A",
                UserFullName = payment.User?.FullName ?? "Unknown",
                UserEmail = payment.User?.Email ?? "Unknown"
            });
        }


    }
}
