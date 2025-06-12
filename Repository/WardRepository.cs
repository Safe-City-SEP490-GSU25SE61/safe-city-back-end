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
    }
}
