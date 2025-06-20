using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Threading.Tasks;

namespace Repository
{
    public class DistrictRepository : GenericRepository<District>, IDistrictRepository
    {
        private readonly AppDbContext _context;

        public DistrictRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<District> GetByNameAsync(string name)
        {
            return await _context.Districts.FirstOrDefaultAsync(d => d.Name == name);
        }
        public async Task<IEnumerable<District>> SearchAsync(string name, int? totalReportedIncidents, int? dangerLevel)
        {
            var query = _context.Districts.AsQueryable();  


            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(d => EF.Functions.Like(d.Name.ToLower(), $"%{name.ToLower()}%"));
            }

           
            if (totalReportedIncidents.HasValue)
            {
                query = query.Where(d => d.TotalReportedIncidents == totalReportedIncidents.Value); 
            }

            
            if (dangerLevel.HasValue)
            {
                query = query.Where(d => d.DangerLevel == dangerLevel.Value); 
            }

            return await query.ToListAsync(); 
        }
    }
}
