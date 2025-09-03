using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using SafeCityBackEnd.Helpers;
using Service.Interfaces;
using System.Net;
using System.Security.Claims;
using static Google.Apis.Requests.BatchRequest;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/subscriptions")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public SubscriptionController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSubscription([FromQuery] int packageId, [FromQuery] string returnUrl, [FromQuery] string cancelUrl)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);

            try
            {
                var result = await _paymentService.CreatePaymentAsync(userId, packageId, returnUrl, cancelUrl);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get transaction information successfully.", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Transaction error: " + ex.Message);
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PayosWebhook([FromBody] WebhookType webhook)
        {
            try
            {
                await _paymentService.HandleWebhookAsync(webhook);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Handled successfully.", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Webhook error: " + ex.Message);
                return StatusCode(500, CustomErrorHandler.SimpleError("Internal server error.", 500));
            }
        }

        [HttpPost("confirm-webhook")]
        public async Task<IActionResult> ConfirmWebhook(string webhookUrl)
        {
            try
            {
                await _paymentService.ConfirmWebhookAsync(webhookUrl);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Confirmed.", null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.InternalServerError, "Failed.", null);
            }

        }


        [HttpGet("status/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> GetSubscriptionPaymentStatus([FromRoute] string orderCode)
        {
            try
            {
                var result = await _paymentService.GetPaymentStatusAsync(orderCode);
                if (result == null)
                    return CustomErrorHandler.SimpleError("Order not found or not paid yet", 404);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Payment status fetched.", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Transaction error: " + ex.Message);
                return StatusCode(500, CustomErrorHandler.SimpleError(ex.Message, 500));
            }
        }

        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetPaymentHistory()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                var result = await _paymentService.GetUserPaymentHistoryAsync(userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Fetched payment history.", result);
            }
            catch (Exception exception)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.InternalServerError, "Failed fetch payment history.", exception.Message);
            }
        }
        [HttpGet("admin/history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPaymentHistory()
        {
            try
            {
                var result = await _paymentService.GetAllPaymentHistoryForAdminAsync();
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Fetched admin payment history", result);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError("Internal server error: " + ex.Message, 500);
            }
        }
        [HttpGet("admin/metrics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminMetrics([FromQuery] string? startMonth,[FromQuery] string? endMonth,[FromQuery] int? monthsBack)
        {
            try
            {
                var stats = await _paymentService.GetAdminRevenueMetricsAsync(startMonth, endMonth, monthsBack);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Fetched revenue & subscription metrics.", stats);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError("Internal server error: " + ex.Message, 500);
            }
        }


    }
}
