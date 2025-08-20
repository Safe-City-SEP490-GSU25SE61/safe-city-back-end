using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class VirtualEscortService : IVirtualEscortService
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IEscortGroupRepository _ecortGroupRepository;

        public VirtualEscortService(IJourneyRepository journeyRepository, IEscortGroupRepository ecortGroupRepository)
        {
            _journeyRepository = journeyRepository;
            _ecortGroupRepository = ecortGroupRepository;
        }

        public async Task<EscortJourney> CreateJourneyFromGoongResponseAsync(Guid userId, CreateJourneyDTO request)
        {
            var isInGroup = await _ecortGroupRepository.IsAlreadyInGroupAsync(userId, request.GroupId);
            if (!isInGroup)
                throw new Exception("Bạn không ở trong nhóm này.");

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
            };          

            return await _journeyRepository.AddAsync(journey);
        }

    }
}
