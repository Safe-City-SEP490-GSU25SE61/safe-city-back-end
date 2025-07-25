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
    public class ProvinceRepository : GenericRepository<Province>,IProvinceRepository
    {
        private readonly AppDbContext _context;

        public ProvinceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
