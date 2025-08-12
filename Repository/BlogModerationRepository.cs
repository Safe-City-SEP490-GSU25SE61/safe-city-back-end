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
    public class BlogModerationRepository : IBlogModerationRepository
    {
        private readonly AppDbContext _context;

        public BlogModerationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BlogModeration moderation)
        {
            _context.BlogModerations.Add(moderation);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<BlogModeration>> GetAllAsync()
        {
            return await _context.BlogModerations.ToListAsync();
        }
    }

}
