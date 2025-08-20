using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository
{
    public class PointHistoryRepository : IPointHistoryRepository
    {
        private readonly AppDbContext _db;
        public PointHistoryRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(PointHistory entity, CancellationToken ct = default)
        {
            await _db.PointHistories.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);
        }

        public IQueryable<PointHistory> Query() => _db.PointHistories.AsNoTracking();

        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

        public async Task<List<PointHistory>> GetByUserAsync(Guid userId, DateTime? fromUtc, string? sourceType,
                                                         bool desc, CancellationToken ct = default)
        {
            var q = _db.PointHistories.AsNoTracking().Where(h => h.UserId == userId);
            if (fromUtc.HasValue) q = q.Where(h => h.CreatedAt >= fromUtc.Value);
            if (!string.IsNullOrWhiteSpace(sourceType)) q = q.Where(h => h.SourceType == sourceType);
            q = desc ? q.OrderByDescending(h => h.CreatedAt) : q.OrderBy(h => h.CreatedAt);
            return await q.ToListAsync(ct);
        }

        public Task<List<PointHistory>> GetBySourceAsync(string sourceType, string sourceId, CancellationToken ct = default)
        {
            return _db.PointHistories.AsNoTracking()
                     .Where(h => h.SourceType == sourceType && h.SourceId == sourceId)
                     .OrderByDescending(h => h.CreatedAt)
                     .ToListAsync(ct);
        }
    }
}
