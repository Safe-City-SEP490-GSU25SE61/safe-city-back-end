using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IJourneyRepository
    {
        Task<EscortJourney> AddAsync(EscortJourney journey);
        Task<EscortJourney?> GetJourneyByIdAsync(int journeyId);
        Task UpdateJourneyAsync(EscortJourney journey);
        Task<EscortJourney> GetActiveJourneyByGroupMemberIdAsync(int memberId);
        Task<List<EscortJourneyDto>> GetJourneysByUserIdAsync(Guid userId);
    }
}
