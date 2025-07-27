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
        Task LikeAsync(Guid userId, int postId);
        Task ApproveBlogAsync(int blogId, bool isApproved, bool isPinned);
        Task TogglePinnedAsync(int blogId, bool pinned);
        Task<BlogResponseDto> CreateBlogAsync(BlogCreateRequestDto request, Guid authorId);
        Task<IEnumerable<BlogResponseDto>> GetBlogsByCommuneAsync(int districtId, Guid currentUserId);
        Task<IEnumerable<BlogResponseDto>> GetCreatedBlogsByUserAsync(Guid userId);
        Task<IEnumerable<BlogResponseForOfficerDto>> GetBlogsForOfficerAsync(Guid userId);
        Task<BlogModerationResponseDto> GetBlogModerationAsync(int id);
        Task<IEnumerable<BlogResponseDto>> GetBlogsByFilterAsync(BlogFilterDto filter, Guid currentUserId);
    }

}
