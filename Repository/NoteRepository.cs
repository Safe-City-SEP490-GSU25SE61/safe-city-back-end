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
    public class NoteRepository : GenericRepository<Note>, INoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetByReportIdAsync(Guid reportId)
        {
            return await _context.Notes
                .Where(n => n.ReportId == reportId)
                .Include(n => n.Officer)
                .ToListAsync();
        }
    }
}
