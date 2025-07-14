using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Threading.Tasks;

namespace Repository
{
    public class WardRepository : GenericRepository<Ward>, IWardRepository
    {
        private readonly AppDbContext _context;

        public WardRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Ward> GetByNameAsync(string name)
        {
            return await _context.Wards.FirstOrDefaultAsync(w => w.Name == name);
        }
        public async Task<IEnumerable<Ward>> SearchAsync(string? name, int? totalReportedIncidents, int? dangerLevel, string? districtName)
        {
            var query = _context.Wards.AsQueryable(); 

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(w => EF.Functions.Like(w.Name.ToLower(), $"%{name.ToLower()}%")); 
            }

            if (totalReportedIncidents.HasValue)
            {
                query = query.Where(w => w.TotalReportedIncidents == totalReportedIncidents.Value);
            }

            if (dangerLevel.HasValue)
            {
                query = query.Where(w => w.DangerLevel == dangerLevel.Value);
            }

            if (!string.IsNullOrEmpty(districtName))
            {
                query = query.Where(w => EF.Functions.Like(w.District.Name.ToLower(), $"%{districtName.ToLower()}%"));
            }

            return await query.ToListAsync();  
        }
        public async Task<IEnumerable<Ward>> GetAllWithDistrictAsync()
        {
            return await _context.Wards
                .Include(w => w.District)
                .ToListAsync();
        }

        public async Task<Ward> GetByIdWithDistrictAsync(int id)
        {
            return await _context.Wards
                .Include(w => w.District)
                .FirstOrDefaultAsync(w => w.Id == id);
        }
        public async Task<Ward?> GetByNameAndDistrictAsync(string name, int districtId)
        {
            var normalizedInput = name.ToLower().Trim();

            return await _context.Wards
                .Where(w => w.DistrictId == districtId)
                .FirstOrDefaultAsync(w => w.Name.ToLower().Trim() == normalizedInput
                                       || w.Name.ToLower().Trim().Contains(normalizedInput));
        }



    }
}

