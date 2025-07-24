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
    public class BlogLikeRepository : IBlogLikeRepository
    {
        private readonly AppDbContext _context;

        public BlogLikeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid userId, int blogId)
        {
            return await _context.Set<BlogLike>()
                .AnyAsync(l => l.UserId == userId && l.BlogId == blogId);
        }

        public async Task<BlogLike?> GetAsync(Guid userId, int blogId)
        {
            return await _context.Set<BlogLike>()
                .FirstOrDefaultAsync(l => l.UserId == userId && l.BlogId == blogId);
        }

        public async Task AddAsync(BlogLike like)
        {
            _context.Set<BlogLike>().Add(like);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(BlogLike like)
        {
            _context.Set<BlogLike>().Remove(like);
            await _context.SaveChangesAsync();
        }
    }
}
