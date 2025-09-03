using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.AspNetCore.SignalR;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class VirtualEscortService : IVirtualEscortService
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IEscortGroupRepository _escortGroupRepository;
        private readonly ILocationHistoryRepository _locationHistoryRepository;

        public VirtualEscortService(IJourneyRepository journeyRepository, IEscortGroupRepository escortGroupRepository, ILocationHistoryRepository locationHistoryRepository)
        {
            _journeyRepository = journeyRepository;
            _escortGroupRepository = escortGroupRepository;
            _locationHistoryRepository = locationHistoryRepository;
        }

        public async Task<EscortJourney> CreateJourneyFromGoongResponseAsync(Guid userId, CreateJourneyDTO request)
        {
            var existedMember = await _escortGroupRepository.GetMemberbyUserIdAndGroupIdAsync(userId, request.GroupId);
            if (existedMember == null)
                throw new Exception("Bạn không ở trong nhóm này.");

            if (existedMember.Account.RemainingVirtualEscorts <= 0)
                throw new InvalidOperationException("Bạn đã hết lượt Virtual Escort.");

            existedMember.Account.RemainingVirtualEscorts -= 1;

            var goongData = JsonDocument.Parse(request.RawJson);

            var startPoint = goongData.RootElement
                .GetProperty("routes")[0]
                .GetProperty("legs")[0]
                .GetProperty("start_address").GetString();

            var startLat = goongData.RootElement
                .GetProperty("routes")[0]
                .GetProperty("legs")[0]
                .GetProperty("start_location")
                .GetProperty("lat").GetDouble();

            var startLng = goongData.RootElement
                .GetProperty("routes")[0]
                .GetProperty("legs")[0]
                .GetProperty("start_location")
                .GetProperty("lng").GetDouble();

            var endPoint = goongData.RootElement
                .GetProperty("routes")[0]
                .GetProperty("legs")[0]
                .GetProperty("end_address").GetString();

            var endLat = goongData.RootElement
                .GetProperty("routes")[0]
                .GetProperty("legs")[0]
                .GetProperty("end_location")
                .GetProperty("lat").GetDouble();

            var endLng = goongData.RootElement
                .GetProperty("routes")[0]
                .GetProperty("legs")[0]
                .GetProperty("end_location")
                .GetProperty("lng").GetDouble();

            var distance = goongData.RootElement
                .GetProperty("routes")[0]
                .GetProperty("legs")[0]
                .GetProperty("distance")
                .GetProperty("value").GetInt32();

            var duration = goongData.RootElement
                .GetProperty("routes")[0]
                .GetProperty("legs")[0]
                .GetProperty("duration")
                .GetProperty("value").GetInt32();

            //var polyline = goongData.RootElement
            //    .GetProperty("routes")[0]
            //    .GetProperty("overview_polyline")
            //    .GetProperty("points").GetString();


            var journey = new EscortJourney
            {
                UserId = userId,
                CreatedInGroupId = request.GroupId,
                StartPoint = startPoint,
                StartLatitude = startLat,
                StartLongitude = startLng,
                EndPoint = endPoint,
                EndLatitude = endLat,
                EndLongitude = endLng,
                DistanceInMeters = distance,
                DurationInSeconds = duration,
                RouteJson = request.RawJson,
                StartTime = DateTime.UtcNow,
                ExpectedEndTime = DateTime.UtcNow.AddSeconds(duration),
                Vehicle = request.Vehicle,
                MemberId = existedMember.Id,
                Watchers = request.WatcherIds.Select(memberId => new EscortJourneyWatcher
                {
                    WatcherId = memberId,
                    AddedAt = DateTime.UtcNow,
                    Status = "Pending"
                }).ToList()
            };

            await _escortGroupRepository.UpdateGroupMemberStatusAsync(existedMember, "on-journey");
            return await _journeyRepository.AddAsync(journey);
        }

        public async Task<EscortJourney> GetJourneyByUserIdAsync(Guid userId, int memberId)
        {
            return await _journeyRepository.GetActiveJourneyByGroupMemberIdAsync(memberId);
        }

        public async Task<string> GetJourneyForObserverAsync(Guid userId, int memberId)
        {
            var journey = await _journeyRepository.GetActiveJourneyByGroupMemberIdAsync(memberId);
            return journey.RouteJson;
        }

        public async Task<JourneyHistoryDto> GetJourneyHistoryAsync(Guid userId)
        {
            var journeys = await _journeyRepository.GetJourneysByUserIdAsync(userId);

            foreach (var journey in journeys)
            {
                journey.StartTime = DateTimeHelper.ToVietnamTime(journey.StartTime.HasValue ? journey.StartTime.Value : DateTime.UtcNow);
                journey.EndTime = DateTimeHelper.ToVietnamTime(journey.EndTime.HasValue ? journey.EndTime.Value : DateTime.UtcNow);
            }

            return new JourneyHistoryDto
            {
                EscortGroupDtos = journeys,
                CanReusePreviousEscortPaths = false,
            };
        }

        public async Task SaveLeaderLocationAsync(int escortJourneyId, Guid leaderId, double lat, double lng, DateTime timestamp)
        {
            var journey = await _journeyRepository.GetJourneyByIdAsync(escortJourneyId);
            if (journey == null)
                throw new Exception("Journey not found");

            if (journey.UserId != leaderId)
                throw new Exception("Only leader can send location updates");

            var location = new LocationHistory
            {
                EscortJourneyId = escortJourneyId,
                Latitude = (decimal)lat,
                Longitude = (decimal)lng,
                RecordedAt = timestamp
            };

            await _locationHistoryRepository.AddAsync(location);
        }

        public async Task<List<LocationHistory>> GetLocationHistoryAsync(int escortJourneyId)
        {
            return await _locationHistoryRepository.GetByJourneyIdAsync(escortJourneyId);
        }

        public async Task EndJourneyAsync(int journeyId)
        {
            var journey = await _journeyRepository.GetJourneyByIdAsync(journeyId);

            if (journey != null)
            {
                journey.Status = "Completed";
                journey.ArrivalTime = DateTime.UtcNow;
                if (journey.StartTime.HasValue)
                {
                    journey.DurationInSeconds =
                        (int)(journey.ArrivalTime.Value - journey.StartTime.Value).TotalSeconds;
                }
                await _journeyRepository.UpdateJourneyAsync(journey);
            }
        }
    }
}
