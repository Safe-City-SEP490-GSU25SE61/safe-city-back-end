using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SosAlertRepository : ISosAlertRepository
    {
        private readonly AppDbContext _context;

        public SosAlertRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateAsync(SosAlert alert)
        {
            await _context.SosAlerts.AddAsync(alert);
            await _context.SaveChangesAsync();
            var fullName = await _context.Accounts
                .Where(a => a.Id == alert.SenderId)
                .Select(a => a.FullName)
                .FirstOrDefaultAsync();

            return fullName;
        }
        public async Task<SosAlert?> GetByIdAsync(int id)
        {
            return await _context.SosAlerts
                .Include(s => s.EscortJourney)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(SosAlert entity)
        {
            _context.SosAlerts.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<SosAlert?> GetLatestBySenderIdAsync(Guid senderId)
        {
            return await _context.SosAlerts
                .Where(a => a.SenderId == senderId)
                .Include(a => a.Sender)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
