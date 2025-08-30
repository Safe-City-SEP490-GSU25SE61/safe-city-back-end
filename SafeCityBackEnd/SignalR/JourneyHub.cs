using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.Interfaces;
using System.Security.Claims;

namespace SafeCityBackEnd.SignalR
{
    public sealed class JourneyHub : Hub
    {
        private readonly IVirtualEscortService _virtualEscortService;
        private readonly ILogger<JourneyHub> _logger;

        public JourneyHub(IVirtualEscortService virtualEscortService, ILogger<JourneyHub> logger)
        {
            _virtualEscortService = virtualEscortService;
            _logger = logger;
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

            await base.OnConnectedAsync();
        }

        public async Task SendLocation(double latitude, double longitude)
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

            await UpdateLocation(latitude, longitude);
        }

        public async Task UpdateLocation(double latitude, double longitude)
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

            if (escort == null) return;

            if (role.ToLower().Equals("leader"))
            {               
                await Clients.Group($"journey-{escort.Id}-observers")
                             .SendAsync("ReceiveLeaderLocation", latitude, longitude);
                _logger.LogInformation($"Observer nhận tọa độ: {latitude}, {longitude}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("User disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
