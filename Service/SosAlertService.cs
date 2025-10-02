using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SosAlertService : ISosAlertService
    {
        private readonly ISosAlertRepository _sosRepo;
        private readonly IWatcherRepository _watcherRepo;
        private readonly IAgoraTokenProvider _tokenProvider;

        public SosAlertService(ISosAlertRepository sosAlertRepository, IWatcherRepository watcherRepo, IAgoraTokenProvider tokenProvider)
        {
            _sosRepo = sosAlertRepository;
            _watcherRepo = watcherRepo;
            _tokenProvider = tokenProvider;
        }

        public async Task<(string senderName, string token, string channelName, int alertId)> CreateAlertAsync(int escortJourneyId, Guid senderId, decimal lat, decimal lng, DateTime timestamp)
        {
            var channelName = $"sos_{escortJourneyId}_{Guid.NewGuid():N}";
            var watchers = await _watcherRepo.GetWatchersByJourneyIdAsync(escortJourneyId);
            var watcherTokens = new List<WatcherTokenInfo>();

            using var trx = await _watcherRepo.BeginTransactionAsync();
            try
            {
                foreach (var w in watchers)
                {
                    //var uid = w.AgoraUid ?? w.WatcherId.ToString();

                    var (token, issuedAt, expireAt, agoraUid) =
                        await _tokenProvider.GenerateRtcTokenAsync(
                            channelName, "0", "GroupVideo", expireInSeconds: 3600, role: 1);

                    w.CallSessionName = channelName;
                    w.Role = w.Role ?? "Watcher";
                    w.JoinTime = null;
                    w.LeaveTime = null;
                    w.CallStatus = "Invited";
                    w.AgoraUid = agoraUid;
                    w.Token = token;
                    w.IssuedAt = issuedAt;
                    w.ExpireAt = expireAt;
                    w.LastRefreshedAt = DateTime.UtcNow;

                    await _watcherRepo.UpdateAsync(w);

                    watcherTokens.Add(new WatcherTokenInfo
                    {
                        WatcherId = w.WatcherId,
                        EscortWatcherRecordId = w.Id,
                        Token = token,
                        AgoraUid = agoraUid,
                        IssuedAt = issuedAt,
                        ExpireAt = expireAt
                    });
                }

                await trx.CommitAsync();
            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }

            var alert = new SosAlert
            {
                EscortJourneyId = escortJourneyId,
                SenderId = senderId,
                Lat = lat,
                Lng = lng,
                Timestamp = timestamp,
                CallChannelName = channelName,
                CallType = "GroupVideo",
                CallStatus = "Ringing",
                CreatedAt = DateTime.UtcNow
            };
            var (senderToken, sIssuedAt, sExpireAt, sAgoraUid) = await _tokenProvider.GenerateRtcTokenAsync(channelName,
                "0", "GroupVideo", expireInSeconds: 3600, role: 1);
            var result = await _sosRepo.CreateAsync(alert);
            return (result.senderName, senderToken, result.channelName, result.alertId);
        }

        public async Task EndSosCallAsync(int sosAlertId)
        {
            var sos = await _sosRepo.GetByIdAsync(sosAlertId);
            if (sos == null) throw new KeyNotFoundException("SosAlert not found");

            sos.CallStatus = "Ended";
            sos.EndedAt = DateTime.UtcNow;

            if (sos.CreatedAt.HasValue)
            {
                sos.CallDuration = sos.EndedAt - sos.CreatedAt;
            }

            await _sosRepo.UpdateAsync(sos);

            var watchers = await _watcherRepo.GetWatchersByJourneyIdAsync(sos.EscortJourneyId);
            foreach (var w in watchers.Where(x => x.CallSessionName == sos.CallChannelName))
            {
                w.CallStatus = "Ended";
                await _watcherRepo.UpdateAsync(w);
            }
        }

        public async Task<(string? channelName, string? token)> JoinWatcherAsync(int sosAlertId, Guid userId)
        {
            var sos = await _sosRepo.GetByIdAsync(sosAlertId) ?? throw new KeyNotFoundException("SosAlert not found");
            var watcher = await _watcherRepo.GetBySosAlertIdAndUserIdAsync(sosAlertId, userId) ?? throw new KeyNotFoundException("Watcher not found");

            watcher.JoinTime = DateTime.UtcNow;
            watcher.CallStatus = "Joined";
            await _watcherRepo.UpdateAsync(watcher);

            if (sos.CallStatus != "Ongoing")
            {
                sos.CallStatus = "Ongoing";
                if (!sos.CreatedAt.HasValue) sos.CreatedAt = DateTime.UtcNow;
                await _sosRepo.UpdateAsync(sos);
            }
            return (watcher.CallSessionName, watcher.Token);
        }

        public async Task LeaveWatcherAsync(int sosAlertId, Guid userId)
        {
            var sos = await _sosRepo.GetByIdAsync(sosAlertId) ?? throw new KeyNotFoundException("SosAlert not found");
            var watcher = await _watcherRepo.GetBySosAlertIdAndUserIdAsync(sosAlertId, userId) ?? throw new KeyNotFoundException("Watcher not found");

            watcher.LeaveTime = DateTime.UtcNow;
            watcher.CallStatus = "Left";
            await _watcherRepo.UpdateAsync(watcher);

            var watchers = await _watcherRepo.GetWatchersByJourneyIdAsync(sos.EscortJourneyId);
            var anyJoined = watchers.Any(w => w.CallSessionName == sos.CallChannelName && w.CallStatus == "Joined");
            if (!anyJoined)
            {
                sos.CallStatus = "Ended";
                sos.EndedAt = DateTime.UtcNow;
                if (sos.CreatedAt.HasValue)
                    sos.CallDuration = sos.EndedAt - sos.CreatedAt;
                await _sosRepo.UpdateAsync(sos);
            }
        }

        public async Task<SosAlert?> GetLatestAlertBySenderIdAsync(Guid senderId)
        {
            return await _sosRepo.GetLatestBySenderIdAsync(senderId);
        }
    }
}
