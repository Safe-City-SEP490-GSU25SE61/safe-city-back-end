using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.Interfaces;
using System.Data;
using System.Security.Claims;

namespace SafeCityBackEnd.SignalR
{
    public sealed class JourneyHub : Hub
    {
        private readonly IVirtualEscortService _virtualEscortService;
        private readonly ISosAlertService _sosAlertService;
        private readonly ILogger<JourneyHub> _logger;

        public JourneyHub(IVirtualEscortService virtualEscortService, ILogger<JourneyHub> logger, ISosAlertService sosAlertService)
        {
            _virtualEscortService = virtualEscortService;
            _logger = logger;
            _sosAlertService = sosAlertService;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Connection aborted: missing userIdClaim");
                Context.Abort();
                return;
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var role = Context.GetHttpContext()?.Request.Query["role"].ToString();
            int.TryParse(Context.GetHttpContext()?.Request.Query["memberId"], out var memberId);

            _logger.LogInformation("User {UserId} attempting to connect. Role={Role}, MemberId={MemberId}",
                                   userId, role, memberId);

            var escort = await _virtualEscortService.GetJourneyByUserIdAsync(userId, memberId);

            if (escort == null)
            {
                _logger.LogWarning("No active journey found for this Member {MemberId}", memberId);
                Context.Abort();
                return;
            }

            if (role.ToLower().Equals("leader"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"journey-{escort.Id}-leader");
                _logger.LogInformation("User {UserId} joined leader group journey-{JourneyId}", userId, escort.Id);
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"journey-{escort.Id}-observers");
                _logger.LogInformation("User {UserId} joined followers group journey-{JourneyId}", userId, escort.Id);
            }

            Context.Items["journeyId"] = escort.Id;

            await base.OnConnectedAsync();
        }

        public async Task SendLocation(double latitude, double longitude, bool isGPSAvailable, bool isInternetAvailable, bool BatteryStatus)
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Connection aborted: missing userIdClaim");
                Context.Abort();
                return;
            }

            var userId = Guid.Parse(userIdClaim.Value);
            _logger.LogWarning($"Leader {userId} gửi tọa độ: {latitude}, {longitude}");

            await UpdateLocation(latitude, longitude, userId, isGPSAvailable, isInternetAvailable, BatteryStatus);
        }

        public async Task UpdateLocation(double latitude, double longitude, Guid userId, bool isGPSAvailable, bool isInternetAvailable, bool BatteryStatus)
        {
            var role = Context.GetHttpContext()?.Request.Query["role"].ToString();
            if (!(Context.Items.TryGetValue("journeyId", out var journeyObj) && journeyObj is int escortJourneyId) || journeyObj == null) return;
            _logger.LogWarning($"journey id: {escortJourneyId}");

            if (role?.ToLower() == "leader")
            {
                await _virtualEscortService.SaveLeaderLocationAsync(escortJourneyId, userId, latitude, longitude, DateTime.UtcNow);
                _logger.LogWarning($"Location history update: {latitude}, {longitude}");

                await Clients.Group($"journey-{escortJourneyId}-observers")
                             .SendAsync("ReceiveLeaderLocation", latitude, longitude, isGPSAvailable, isInternetAvailable, BatteryStatus);
                _logger.LogInformation($"Observer nhận tọa độ: {latitude}, {longitude}");
            }
        }


        public async Task SendSos(decimal lat, decimal lng, DateTime timestamp)
        {
            if (!(Context.Items.TryGetValue("journeyId", out var journeyObj) && journeyObj is int escortJourneyId) || journeyObj == null)
                throw new HubException("No journey found for this connection");

            Guid senderId = Guid.Parse(Context.UserIdentifier);

            var senderName = await _sosAlertService.CreateAlertAsync(escortJourneyId, senderId, lat, lng, timestamp);
            _logger.LogWarning($"SoS Alert: {lat}, {lng} . TimeStamp: {timestamp}");

            await Clients.Group($"journey-{escortJourneyId}-observers").SendAsync("ReceiveSos",
                $"{senderName} hiện đang gửi tín hiệu cầu cứu.", (double)lat, (double)lng);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("User disconnected: {ConnectionId}", Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task EndJourney()
        {
            if (Context.Items.TryGetValue("journeyId", out var journeyObj) && journeyObj is int escortJourneyId)
            {
                var role = Context.GetHttpContext()?.Request.Query["role"].ToString();

                if (role?.ToLower() == "leader")
                {
                    await _virtualEscortService.EndJourneyAsync(escortJourneyId);

                    await Clients.Group($"journey-{escortJourneyId}-observers")
                                 .SendAsync("LeaderDisconnected", "Hành trình đã kết thúc.");

                    _logger.LogInformation($"Leader của journey {escortJourneyId} đã kết thúc hành trình, thông báo tới observers.");
                }
                else
                {
                    _logger.LogWarning($"EndJourney bị gọi bởi user không phải leader. Role: {role}");
                    throw new HubException("Chỉ leader mới có thể kết thúc hành trình.");
                }
            }
        }
    }
}
