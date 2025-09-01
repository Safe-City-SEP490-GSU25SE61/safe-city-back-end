using BusinessObject.Models;
using DataAccessLayer.DataContext;
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

        public async Task<SosAlert> CreateAsync(SosAlert alert)
        {
            await _context.SosAlerts.AddAsync(alert);
            await _context.SaveChangesAsync();
            return alert;
        }
    }
}
