using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Google;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogLikeRepository _likeRepository;
        private readonly IBlogMediaRepository _mediaRepository;
        private readonly IFirebaseStorageService _fileUploader;

        public BlogService(
            IBlogRepository blogRepository,
            IBlogLikeRepository likeRepository,
            IBlogMediaRepository mediaRepository,
            IFirebaseStorageService fileUploader)
        {
            _blogRepository = blogRepository;
            _likeRepository = likeRepository;
            _mediaRepository = mediaRepository;
            _fileUploader = fileUploader;
        }

        public async Task LikeAsync(Guid userId, int postId)
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

        public async Task<IEnumerable<BlogResponseDto>> GetCreatedBlogsByUserAsync(Guid userId)
        {
            return await _blogRepository.GetCreatedBlogsByUserAsync(userId);
        }

        public async Task ApproveBlogAsync(int blogId, bool isApproved, bool isPinned)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            blog.IsApproved = isApproved;
            blog.Pinned = isApproved && isPinned;
            blog.UpdatedAt = DateTime.UtcNow;

            await _blogRepository.UpdateAsync(blog);
        }

        public async Task TogglePinnedAsync(int blogId, bool pinned)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            blog.Pinned = pinned;
            blog.UpdatedAt = DateTime.UtcNow;

            await _blogRepository.UpdateAsync(blog);
        }

        public async Task<BlogResponseDto> CreateBlogAsync(BlogCreateRequestDto request, Guid authorId)
        {
            var blog = new Blog
            {
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                DistrictId = request.DistrictId,
                AuthorId = authorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _blogRepository.AddAsync(blog);

            int slot = 1;
            foreach (var file in request.MediaFiles.Take(4))
            {
                var url = await _fileUploader.UploadFileAsync(file, "uploads");

                var media = new BlogMedia
                {
                    BlogId = blog.Id,
                    FileUrl = url,
                    Type = file.ContentType.Contains("video") ? "video" : "image",
                    MediaSlot = slot++,
                    CreatedAt = DateTime.UtcNow
                };

                await _mediaRepository.AddAsync(media);
            }

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

        public async Task<IEnumerable<BlogResponseDto>> GetBlogsByDistrictAsync(int districtId, Guid currentUserId)
        {
            var blogs = await _blogRepository.GetVisibleByDistrictAsync(districtId, currentUserId);

            foreach (var blog in blogs)
            {
                blog.MediaUrls = await _mediaRepository.GetUrlsByPostIdAsync(blog.Id);
            }

            return blogs;
        }
    }

}
