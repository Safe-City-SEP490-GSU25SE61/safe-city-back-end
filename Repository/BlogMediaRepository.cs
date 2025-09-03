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
    public class BlogMediaRepository : IBlogMediaRepository
    {
        private readonly AppDbContext _context;

        public BlogMediaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BlogMedia media)
        {
            _context.BlogMedias.Add(media);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetUrlsByPostIdAsync(int blogId)
        {
            return await _context.BlogMedias
                .Where(m => m.BlogId == blogId)
                .OrderBy(m => m.MediaSlot)
                .Select(m => m.FileUrl)
                .ToListAsync();
        }
    }
}
