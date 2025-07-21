using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Threading.Tasks;

namespace Repository
{
    public class CommuneRepository : GenericRepository<Commune>, ICommuneRepository
    {
        private readonly AppDbContext _context;

        public CommuneRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Commune> GetByNameAsync(string name)
        {
            return await _context.Communes
        .FirstOrDefaultAsync(d => d.Name.ToLower() == name.ToLower());
        }
        public async Task<IEnumerable<Commune>> SearchAsync(string name, int? totalReportedIncidents)
        {
            var query = _context.Communes.AsQueryable();  


            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(d => EF.Functions.Like(d.Name.ToLower(), $"%{name.ToLower()}%"));
            }

           
            if (totalReportedIncidents.HasValue)
            {
                query = query.Where(d => d.TotalReportedIncidents == totalReportedIncidents.Value); 
            }

            

            return await query.ToListAsync(); 
        }
    }
}
