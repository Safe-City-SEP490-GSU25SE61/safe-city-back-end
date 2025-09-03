using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class IncidentReportRepository : IIncidentReportRepository
    {
        private readonly AppDbContext _context;

        public IncidentReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IncidentReport?> GetByIdAsync(Guid id)
        {
            return await _context.IncidentReports
                .Include(r => r.User)
                .Include(r => r.Verifier)
                .Include(r => r.Commune)
                .Include(r => r.Notes)
                .ThenInclude(n => n.Officer)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<IncidentReport>> GetAllAsync()
        {
            return await _context.IncidentReports
                .Include(r => r.User)
                .Include(r => r.Verifier)
                .Include(r => r.Commune)
                .Include(r => r.Notes)
                .ToListAsync();
        }

        public async Task CreateAsync(IncidentReport report)
        {
            await _context.IncidentReports.AddAsync(report);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(Guid id, string status, Guid officerId)
        {
            var report = await _context.IncidentReports.FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) throw new KeyNotFoundException("Report not found");

            report.Status = status;
            report.VerifiedBy = officerId;
            await _context.SaveChangesAsync();
        }
        public async Task UpdateStatusByUserAsync(Guid reportId, string status)
        {
            var report = await _context.IncidentReports.FirstOrDefaultAsync(r => r.Id == reportId);
            if (report == null) throw new KeyNotFoundException("Report not found");

            report.Status = status;
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(IncidentReport report)
        {
            _context.IncidentReports.Update(report);
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<IncidentReport>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            if (ids == null) return new List<IncidentReport>();
            var list = ids.Distinct().ToList();
            if (list.Count == 0) return new List<IncidentReport>();

            return await _context.IncidentReports
                .AsNoTracking()
                .Where(r => list.Contains(r.Id))
                .Select(r => new IncidentReport
                {
                    Id = r.Id,
                    Status = r.Status,
                    OccurredAt = r.OccurredAt,
                    Address = r.Address
                })
                .ToListAsync();
        }

    }

}
