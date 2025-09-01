using BusinessObject.Models;
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
        private readonly ISosAlertRepository _sosAlertRepository;

        public SosAlertService(ISosAlertRepository sosAlertRepository)
        {
            _sosAlertRepository = sosAlertRepository;
        }

        public async Task<string> CreateAlertAsync(int escortJourneyId, Guid senderId, decimal lat, decimal lng, DateTime timestamp)
        {
            var alert = new SosAlert
            {
                EscortJourneyId = escortJourneyId,
                SenderId = senderId,
                Lat = lat,
                Lng = lng,
                Timestamp = timestamp
            };

            return await _sosAlertRepository.CreateAsync(alert);
        }
    }
}
