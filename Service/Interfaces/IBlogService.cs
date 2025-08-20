using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IBlogService
    {
        Task Like(Guid userId, int postId);
        Task ApproveBlog(int blogId, bool isApproved, bool isPinned, Guid officerId);
        Task TogglePinned(int blogId, bool pinned);
        Task<BlogResponseDto> CreateBlog(BlogCreateRequestDto request, Guid authorId);
        Task<IEnumerable<BlogResponseDto>> GetBlogsByCommune(int districtId, Guid currentUserId);
        Task<IEnumerable<BlogResponseDto>> GetCreatedBlogsByUser(Guid userId);
        Task<IEnumerable<BlogResponseForOfficerDto>> GetBlogsForOfficer(Guid userId, BlogFilterForOfficerRequest filter);
        Task<BlogModerationResponseDto> GetBlogModeration(int id);
        Task<FollowingRequestBlogResponseDto> GetBlogsByFilter(BlogFilterDto filter, Guid currentUserId);
        Task UpdateBlogVisibility(int blogId, bool isVisible);
        Task<FirstRequestBlogResponseDto> GetFirstRequestData(Guid currentUserId);
        Task<object> GetBlogMetricsAsync(int? communeId, string? startMonth, string? endMonth, int? monthsBack);

    }

}
