using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Repository.Interfaces
{
    public interface IPointHistoryRepository
    {
        Task AddAsync(PointHistory entity, CancellationToken ct = default);
        IQueryable<PointHistory> Query(); 
        Task SaveChangesAsync(CancellationToken ct = default);

        Task<List<PointHistory>> GetByUserAsync(Guid userId, DateTime? fromUtc, string? sourceType,
                                            bool desc, CancellationToken ct = default);


        Task<List<PointHistory>> GetBySourceAsync(string sourceType, string sourceId, CancellationToken ct = default);
    }
}

