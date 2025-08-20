using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Service.Helpers;
using DataAccessLayer.DataContext;
using Google;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogLikeRepository _likeRepository;
        private readonly IBlogMediaRepository _mediaRepository;
        private readonly IFirebaseStorageService _fileUploader;
        private readonly IBlogModerationRepository _blogModerationRepository;
        private readonly BlogModerationService _blogModerationService;
        private readonly IAccountRepository _accountRepository;
        private readonly IProvinceRepository _provinceRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public BlogService(
            IBlogRepository blogRepository,
            IBlogLikeRepository likeRepository,
            IBlogMediaRepository mediaRepository,
            IFirebaseStorageService fileUploader,
            IBlogModerationRepository blogModerationRepository,
            BlogModerationService blogModerationService,
            IAccountRepository accountRepository,
            IProvinceRepository provinceRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _blogRepository = blogRepository;
            _likeRepository = likeRepository;
            _mediaRepository = mediaRepository;
            _fileUploader = fileUploader;
            _blogModerationRepository = blogModerationRepository;
            _blogModerationService = blogModerationService;
            _accountRepository = accountRepository;
            _provinceRepository = provinceRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task Like(Guid userId, int postId)
        {
            var existedlike = await _likeRepository.GetAsync(userId, postId);
            if (existedlike != null)
                await _likeRepository.RemoveAsync(existedlike);
            else
            {
                var like = new BlogLike
                {
                    UserId = userId,
                    BlogId = postId,
                    LikedAt = DateTime.UtcNow
                };
                await _likeRepository.AddAsync(like);
            }
        }

        public async Task<IEnumerable<BlogResponseDto>> GetCreatedBlogsByUser(Guid userId)
        {
            var blogs = await _blogRepository.GetCreatedBlogsByUserAsync(userId);
            foreach (var blog in blogs)
            {
                blog.CreatedAt = DateTimeHelper.ToVietnamTime(blog.CreatedAt);
            }
            return blogs;
        }

        public async Task ApproveBlog(Guid userId, int blogId, bool isApproved, bool isPinned)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            blog.IsApproved = isApproved;
            if (isApproved && isPinned)
            {
                var pinnedCount = await _blogRepository.GetBlogsPinnedNumberAsync(blog.CommuneId);
                if (pinnedCount >= 3)
                    throw new Exception("Số lượng bài ghim tối đa là 3. Vui lòng bỏ ghim bớt trước khi ghim bài mới.");
            }

            blog.ApprovedBy = userId;
            blog.Pinned = isApproved && isPinned;
            blog.IsVisible = isApproved;
            blog.UpdatedAt = DateTime.UtcNow;

            await _blogRepository.UpdateAsync(blog);
        }

        public async Task TogglePinned(int blogId, bool pinned)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            blog.Pinned = pinned;
            blog.UpdatedAt = DateTime.UtcNow;

            await _blogRepository.UpdateAsync(blog);
        }

        public async Task<BlogResponseDto> CreateBlog(BlogCreateRequestDto request, Guid authorId)
        {
            var images = request.MediaFiles?.Where(f => f.ContentType.StartsWith("image")).ToList() ?? new();
            var videos = request.MediaFiles?.Where(f => f.ContentType.StartsWith("video")).ToList() ?? new();

            if (images.Count > 10)
                throw new ArgumentException("You can upload up to 10 images only.");

            if (images.Any(i => i.Length > 8 * 1024 * 1024))
                throw new ArgumentException("Each image must not exceed 8MB.");

            if (videos.Count > 1)
                throw new ArgumentException("You can upload only 1 video.");

            if (videos.Any(v => v.Length > 400 * 1024 * 1024))
                throw new ArgumentException("Video must not exceed 400MB.");

            var blog = new Blog
            {
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                CommuneId = request.CommuneId,
                AuthorId = authorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _blogRepository.AddAsync(blog);

            int slot = 1;
            foreach (var file in request.MediaFiles.Take(11))
            {
                var url = await _fileUploader.UploadFileAsync(file, "uploads");

                var media = new BlogMedia
                {
                    BlogId = blog.Id,
                    FileUrl = url,
                    Type = file.ContentType.StartsWith("video") ? "video" : "image",
                    MediaSlot = slot++,
                    CreatedAt = DateTime.UtcNow
                };

                await _mediaRepository.AddAsync(media);
            }

            var result = await _blogModerationService.ModerateBlogAsync(blog.Title, blog.Content, blog.Type);

            var moderation = new BlogModeration
            {
                BlogId = blog.Id,
                IsApproved = result.IsApproved,
                Politeness = result.Politeness,
                NoAntiState = result.NoAntiState,
                PositiveMeaning = result.PositiveMeaning,
                TypeRequirement = result.TypeRequirement,
                Reasoning = result.Reasoning,
                ViolationsJson = JsonSerializer.Serialize(result.Violations, new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }),
                CreatedAt = DateTime.UtcNow
            };

            await _blogModerationRepository.AddAsync(moderation); 

            return new BlogResponseDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                Type = blog.Type,
                CreatedAt = blog.CreatedAt,
                MediaUrls = await _mediaRepository.GetUrlsByPostIdAsync(blog.Id)
            };
        }

        public async Task<IEnumerable<BlogResponseDto>> GetBlogsByCommune(int CommuneId, Guid currentUserId)
        {
            var blogs = await _blogRepository.GetVisibleByCommuneAsync(CommuneId, currentUserId);

            foreach (var blog in blogs)
            {
                blog.MediaUrls = await _mediaRepository.GetUrlsByPostIdAsync(blog.Id);
            }

            return blogs;
        }

        public async Task<IEnumerable<BlogResponseForOfficerDto>> GetBlogsForOfficer(Guid userId, BlogFilterForOfficerRequest filter)
        {
            var user = await _accountRepository.GetByIdAsync(userId);
            var communeId = user.CommuneId ?? -1;
            var blogs = await _blogRepository.GetBlogsForOfficerAsync(communeId, filter);
            foreach (var blog in blogs)
            {
                blog.CreatedAt = DateTimeHelper.ToVietnamTime(blog.CreatedAt);
            }
            return blogs;
        }


        public async Task<BlogModerationResponseDto> GetBlogModeration(int id)
        {
            var blogModeration = await _blogRepository.GetDetailByIdAsync(id);
            blogModeration.BlogModeration.CreatedAt = DateTimeHelper.ToVietnamTime(blogModeration.BlogModeration.CreatedAt);
            blogModeration.CreatedAt = DateTimeHelper.ToVietnamTime(blogModeration.CreatedAt);
            return blogModeration;
        }

        public async Task<FollowingRequestBlogResponseDto> GetBlogsByFilter(BlogFilterDto filter, Guid currentUserId)
        {
            var blogs = await _blogRepository.GetBlogsByFilterAsync(filter, currentUserId);

            foreach (var blog in blogs)
            {
                blog.MediaUrls = await _mediaRepository.GetUrlsByPostIdAsync(blog.Id);
                blog.CreatedAt = DateTimeHelper.ToVietnamTime(blog.CreatedAt);
            }

            return new FollowingRequestBlogResponseDto
            {
                Blogs = blogs,
                IsPremium = await _subscriptionRepository.GetActiveByUserIdAsync(currentUserId) != null,
            };
        }

        public async Task UpdateBlogVisibility(int blogId, bool isVisible)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);

            blog.IsVisible = isVisible;
            blog.UpdatedAt = DateTime.UtcNow;

            await _blogRepository.UpdateAsync(blog);
        }

        public async Task<FirstRequestBlogResponseDto> GetFirstRequestData(Guid currentUserId)
        {
            var blogs = await _blogRepository.GetBlogsFirstRequestAsync(currentUserId);

            foreach (var blog in blogs)
            {
                blog.CreatedAt = DateTimeHelper.ToVietnamTime(blog.CreatedAt);
            }
            return new FirstRequestBlogResponseDto
            {
                Provinces = await _provinceRepository.GetAllProAsync(),
                Blogs = blogs,
                IsPremium = await _subscriptionRepository.GetActiveByUserIdAsync(currentUserId) != null,
            };
        }

        public async Task<object> GetBlogMetricsAsync(int? communeId, string? startMonth, string? endMonth, int? monthsBack)
        {

            var allBlogs = await _blogRepository.GetAllAsync();                
            var allModerations = await _blogModerationRepository.GetAllAsync();


            static DateTime? ParseMonth(string? ym)
            {
                if (string.IsNullOrWhiteSpace(ym)) return null;
                if (DateTime.TryParseExact(ym, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out var d))
                    return new DateTime(d.Year, d.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                return null;
            }

            var start = ParseMonth(startMonth);
            var end = ParseMonth(endMonth);


            if (!start.HasValue || !end.HasValue)
            {
                var todayStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                int back = monthsBack.GetValueOrDefault(6);
                start = todayStart.AddMonths(-back + 1);
                end = todayStart; 
            }


            static bool InRange(DateTime dt, DateTime startMonthStart, DateTime endMonthStart)
            {
                var rangeStart = new DateTime(startMonthStart.Year, startMonthStart.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var rangeEnd = new DateTime(endMonthStart.Year, endMonthStart.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
                return dt >= rangeStart && dt < rangeEnd;
            }


            var blogs = allBlogs
                .Where(b => (!communeId.HasValue || b.CommuneId == communeId.Value)
                         && InRange(b.CreatedAt, start!.Value, end!.Value))
                .ToList();


            var moderationByBlogId = allModerations
                .GroupBy(m => m.BlogId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First()); 


            var totalCreated = blogs.Count; 
                                            
            bool IsViolation(int blogId)
                => moderationByBlogId.TryGetValue(blogId, out var m)
                   && (!m.IsApproved || !string.IsNullOrWhiteSpace(m.ViolationsJson));

            var totalViolations = blogs.Count(b => IsViolation(b.Id));

 
            var approved = blogs.Count(b => b.IsApproved);
            var pending = blogs.Count(b => !b.IsApproved && !IsViolation(b.Id)); 
            var hidden = blogs.Count(b => !b.IsVisible);
            var visible = blogs.Count(b => b.IsVisible);
            var pinned = blogs.Count(b => b.Pinned);

      
            var byType = blogs
                .GroupBy(b => b.Type.ToString())
                .Select(g => new { type = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .ToList();

 
            var topAuthors = blogs
                .GroupBy(b => b.AuthorId)
                .Select(g => new { authorId = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .Take(5)
                .ToList();

    
            var months = new List<DateTime>();
            for (var cur = new DateTime(start.Value.Year, start.Value.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                 cur <= new DateTime(end.Value.Year, end.Value.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                 cur = cur.AddMonths(1))
            {
                months.Add(cur);
            }

            static bool InMonth(DateTime dt, DateTime monthStart)
            {
                var ms = new DateTime(monthStart.Year, monthStart.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var me = ms.AddMonths(1);
                return dt >= ms && dt < me;
            }

            var monthlyCreated = months.Select(m => new {
                month = m.ToString("yyyy-MM"),
                count = blogs.Count(b => InMonth(b.CreatedAt, m))
            }).ToList();

            var monthlyViolations = months.Select(m => new {
                month = m.ToString("yyyy-MM"),
                count = blogs.Count(b => InMonth(b.CreatedAt, m) && IsViolation(b.Id))
            }).ToList();

            return new
            {
                filter = new
                {
                    communeId,
                    startMonth = start.Value.ToString("yyyy-MM"),
                    endMonth = end.Value.ToString("yyyy-MM")
                },
                totals = new
                {
                    created = totalCreated,
                    violations = totalViolations
                },
                status = new
                {
                    approved,
                    pending,
                    visible,
                    hidden,
                    pinned
                },
                byType,
                topAuthors,
                monthly = new
                {
                    created = monthlyCreated,
                    violations = monthlyViolations
                }
            };
        }

    }

}
