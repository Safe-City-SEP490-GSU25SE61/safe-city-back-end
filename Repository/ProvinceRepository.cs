using BusinessObject.DTOs.ResponseModels;
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
    public class ProvinceRepository : GenericRepository<Province>,IProvinceRepository
    {
        private readonly AppDbContext _context;

        public ProvinceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProvinceDto>> GetAllProAsync()
        {
            var provinces = await _context.Provinces
         .Include(p => p.Communes)
         .Select(p => new ProvinceDto
         {
             Id = p.Id,
             Name = p.Name,
             Communes = p.Communes.Select(c => new CommuneForCitizenDTO
             {
                 Id = c.Id,
                 Name = c.Name
             }).ToList()
         })
         .ToListAsync();
            return provinces;
        }
    }
}
