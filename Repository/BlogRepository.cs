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
                    CommentNumber = b.Comments.Count(),
                    LikeNumber = b.Likes.Count(),
                    IsApproved = b.IsApproved,
                    IsVisible = b.IsVisible,
                    Pinned = b.Pinned,
                    ApprovedBy = b.ApprovedByAccount != null ? b.ApprovedByAccount.FullName : null,
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
                    AvatarUrl = b.Author.ImageUrl,
                    UserRank = b.Author.Achievement != null ? b.Author.Achievement.Name : "Unrank",
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
                    AvatarUrl = b.Author.ImageUrl,
                    UserRank = b.Author.Achievement != null ? b.Author.Achievement.Name : "Unrank",
                    CreatedAt = b.CreatedAt,
                    TotalLike = b.Likes.Count,
                    TotalComment = b.Comments.Count,
                    IsLike = b.Likes.Any(l => l.UserId == userId)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogResponseForOfficerDto>> GetBlogsForOfficerAsync(int communeId, BlogFilterForOfficerRequest filter)
        {
            var query = _context.Set<Blog>()
                .Where(b => b.CommuneId == communeId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(b => b.Title.ToLower().Contains(filter.Keyword.ToLower()));
            }

            if (filter.Type.HasValue)
            {
                query = query.Where(b => b.Type == filter.Type.Value);
            }

            if (filter.SortOption.Equals("newest"))
            {
                query = query.OrderByDescending(b => b.CreatedAt);
            }
            else if (filter.SortOption.Equals("oldest"))
            {
                query = query.OrderBy(b => b.CreatedAt);
            }
            else // default sort: pinned > not approved > approved
            {
                query = query
                    .OrderByDescending(b => b.Pinned)
                    .ThenBy(b => b.IsApproved)
                    .ThenByDescending(b => b.CreatedAt);
            }

            return await query
                .Select(b => new BlogResponseForOfficerDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author.FullName,
                    Type = b.Type,
                    CreatedAt = b.CreatedAt,
                    IsApproved = b.IsApproved,
                    IsVisible = b.IsVisible,
                    Pinned = b.Pinned,
                    ApprovedBy = b.ApprovedByAccount != null ? b.ApprovedByAccount.FullName : null,
                })
                .ToListAsync();
        }

        public async Task<int> GetBlogsPinnedNumberAsync(int communeId)
        {
            return await _context.Set<Blog>()
                .Where(b => b.CommuneId == communeId && b.Pinned)
                .Take(4)
                .CountAsync();

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
                    AvatarUrl = b.Author.ImageUrl,
                    UserRank = b.Author.Achievement != null ? b.Author.Achievement.Name : "Unrank",
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

        public async Task<IEnumerable<BlogResponseDto>> GetBlogsFirstRequestAsync(Guid currentUserId)
        {
            var blogs = await _context.Blogs
                .Where(b => b.IsVisible && b.IsApproved)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BlogResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    AuthorName = b.Author.FullName,
                    CommuneName = b.Commune.Name,
                    ProvinceName = b.Commune.Province.Name,
                    Pinned = b.Pinned,
                    Type = b.Type,
                    AvatarUrl = b.Author.ImageUrl,
                    UserRank = b.Author.Achievement != null ? b.Author.Achievement.Name : "Unrank",
                    CreatedAt = b.CreatedAt,
                    TotalLike = b.Likes.Count,
                    TotalComment = b.Comments.Count,
                    IsLike = b.Likes.Any(l => l.UserId == currentUserId)
                })
                .Take(10)
                .ToListAsync();

            return blogs;
        }
        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            return await _context.Blogs
                .Include(b => b.Moderation)
                .ToListAsync();
        }
        
        public async Task<List<Blog>> GetByIdsAsync(IEnumerable<int> ids)
        {
            if (ids == null) return new List<Blog>();
            var list = ids.Distinct().ToList();
            if (list.Count == 0) return new List<Blog>();

            return await _context.Blogs
                .AsNoTracking()
                .Where(b => list.Contains(b.Id))
                .Select(b => new Blog
                {
                    Id = b.Id,
                    Title = b.Title
                    
                })
                .ToListAsync();
        }

    }

}
