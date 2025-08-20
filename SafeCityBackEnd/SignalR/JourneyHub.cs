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

        public JourneyHub(IVirtualEscortService virtualEscortService)
        {
            _virtualEscortService = virtualEscortService;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                Context.Abort();
                return;
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var role = Context.GetHttpContext()?.Request.Query["role"].ToString();
            var memberId = Context.GetHttpContext()?.Request.Query["memberId"].ToString();

            var escort = await _virtualEscortService.GetJourneyByUserIdAsync(userId);

            if (string.IsNullOrEmpty(memberId))
            {
                Context.Abort();
                return;
            }

            if (role == "leader")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"journey-{escort.Id}-leader");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"journey-{escort.Id}-followers");
            }

            await base.OnConnectedAsync();
        }

        public async Task UpdateLocation(double latitude, double longitude)
        {
            await Clients.OthersInGroup(Context.ConnectionId)
                         .SendAsync("ReceiveLocationUpdate", latitude, longitude);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
