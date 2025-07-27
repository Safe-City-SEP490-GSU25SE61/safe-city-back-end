using BusinessObject.DTOs.RequestModels;
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
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext _context;

        public BlogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await _context.Set<Blog>().FindAsync(id);
        }

        public async Task<BlogModerationResponseDto> GetDetailByIdAsync(int id)
        {
            var blogDto = await _context.Blogs
                .Where(b => b.Id == id)
                .Select(b => new BlogModerationResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    Type = b.Type,
                    AuthorName = b.Author.FullName,
                    CreatedAt = b.CreatedAt,
                    MediaUrls = b.Media.OrderBy(m => m.MediaSlot).Select(m => m.FileUrl).ToList(),
                    BlogModeration = b.Moderation
                })
                .FirstOrDefaultAsync();

            return blogDto;
        }


        public async Task<int> AddAsync(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return blog.Id;
        }

        public async Task UpdateAsync(Blog blog)
        {
            _context.Set<Blog>().Update(blog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BlogResponseDto>> GetVisibleByCommuneAsync(int districtId, Guid currentUserId)
        {
            var blogs = await _context.Set<Blog>()
                .Where(b => b.CommuneId == districtId && b.IsVisible && b.IsApproved)
                .OrderByDescending(b => b.Pinned)
                .ThenByDescending(b => b.CreatedAt)
                .Select(b => new BlogResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    AuthorName = b.Author.FullName,
                    CommuneName = b.Commune.Name,
                    Pinned = b.Pinned,
                    Type = b.Type,
                    AvaterUrl = b.Author.ImageUrl,
                    CreatedAt = b.CreatedAt,
                    TotalLike = b.Likes.Count,
                    TotalComment = b.Comments.Count,
                    IsLike = b.Likes.Any(l => l.UserId == currentUserId)
                })
                .ToListAsync();

            return blogs;
        }

        public async Task<IEnumerable<BlogResponseDto>> GetCreatedBlogsByUserAsync(Guid userId)
        {
            return await _context.Blogs
                .Where(b => b.AuthorId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BlogResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    AuthorName = b.Author.FullName,
                    CommuneName = b.Commune.Name,
                    Pinned = b.Pinned,
                    Type = b.Type,
                    AvaterUrl = b.Author.ImageUrl,
                    CreatedAt = b.CreatedAt,
                    TotalLike = b.Likes.Count,
                    TotalComment = b.Comments.Count,
                    IsLike = b.Likes.Any(l => l.UserId == userId)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogResponseForOfficerDto>> GetBlogsForOfficerAsync(int communeId)
        {
            var blogs = await _context.Set<Blog>()
                .Where(b => b.CommuneId == communeId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BlogResponseForOfficerDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author.FullName,
                    Type = b.Type,
                    CreatedAt = b.CreatedAt,
                })
                .ToListAsync();

            return blogs;
        }

        public async Task<IEnumerable<BlogResponseDto>> GetBlogsByFilterAsync(BlogFilterDto filter, Guid currentUserId)
        {
            var query = _context.Blogs
                .Where(b => b.IsVisible && b.IsApproved)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                query = query.Where(b => b.Title.ToLower().Contains(filter.Title.ToLower()));
            }

            if (filter.CommuneId.HasValue)
            {
                query = query.Where(b => b.CommuneId == filter.CommuneId.Value);
            }

            if (filter.Type.HasValue)
            {
                query = query.Where(b => b.Type == filter.Type.Value);
            }

            var blogs = await query
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BlogResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    AuthorName = b.Author.FullName,
                    CommuneName = b.Commune.Name,
                    Pinned = b.Pinned,
                    Type = b.Type,
                    AvaterUrl = b.Author.ImageUrl,
                    CreatedAt = b.CreatedAt,
                    TotalLike = b.Likes.Count,
                    TotalComment = b.Comments.Count,
                    IsLike = b.Likes.Any(l => l.UserId == currentUserId)
                })
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return blogs;
        }

    }

}
