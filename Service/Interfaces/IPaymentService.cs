using BusinessObject.DTOs.ResponseModels;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPaymentService
    {
        Task<CreatePaymentResult> CreatePaymentAsync(Guid userId, int packageId, string returnUrl, string cancelUrl);
        Task HandleWebhookAsync(WebhookType webhookBody);
        Task<PaymentStatusResponse?> GetPaymentStatusAsync(string orderCode);
        Task ConfirmWebhookAsync(string webhookUrl);
        Task<IEnumerable<PaymentHistoryResponseModel>> GetUserPaymentHistoryAsync(Guid userId);
        Task<IEnumerable<AdminPaymentHistoryResponseModel>> GetAllPaymentHistoryForAdminAsync();
        Task<object> GetAdminRevenueMetricsAsync(string? startMonth, string? endMonth, int? monthsBack);


    }
}
