using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IChangeHistoryService
    {
        Task LogChangeAsync(string entityType, string entityId, string fieldName, string oldValue, string newValue);
        Task<IEnumerable<ChangeHistoryDTO>> GetHistoryByEntityAsync(string entityType, string entityId);
    }

}
