using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IVirtualEscortService
    {
        Task<EscortJourney> CreateJourneyFromGoongResponseAsync(Guid userId, CreateJourneyDTO request);
    }
}
