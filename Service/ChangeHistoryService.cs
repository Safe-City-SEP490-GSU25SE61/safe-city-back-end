using BusinessObject.DTOs.ResponseModels;
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
    public class ChangeHistoryService : IChangeHistoryService
    {
        private readonly IChangeHistoryRepository _repo;

        public ChangeHistoryService(IChangeHistoryRepository repo)
        {
            _repo = repo;
        }

        public async Task LogChangeAsync(string entityType, string entityId, string fieldName, string oldValue, string newValue)
        {
            if (oldValue == newValue) return;

            var history = new ChangeHistory
            {
                EntityType = entityType,
                EntityId = entityId,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedAt = DateTime.UtcNow
            };

            await _repo.CreateAsync(history);
        }

        public async Task<IEnumerable<ChangeHistoryDTO>> GetHistoryByEntityAsync(string entityType, string entityId)
        {
            var items = await _repo.GetByEntityAsync(entityType, entityId);
            return items.Select(h => new ChangeHistoryDTO
            {
                FieldName = h.FieldName,
                OldValue = h.OldValue,
                NewValue = h.NewValue,
                ChangedAt = h.ChangedAt
            }).ToList();
        }
    }

}
