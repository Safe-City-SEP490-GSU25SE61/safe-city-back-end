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
    }
}
