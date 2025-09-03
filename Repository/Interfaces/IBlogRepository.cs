using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IBlogRepository
    {
        Task<Blog?> GetByIdAsync(int id);
        Task<int> AddAsync(Blog blog);
        Task UpdateAsync(Blog blog);
        Task<int> GetBlogsPinnedNumberAsync(int communeId);
        Task<BlogModerationResponseDto> GetDetailByIdAsync(int id);
        Task<IEnumerable<BlogResponseDto>> GetVisibleByCommuneAsync(int districtId, Guid currentUserId);
        Task<IEnumerable<BlogResponseDto>> GetCreatedBlogsByUserAsync(Guid userId);
        Task<IEnumerable<BlogResponseForOfficerDto>> GetBlogsForOfficerAsync(int communeId, BlogFilterForOfficerRequest filter);
        Task<IEnumerable<BlogResponseDto>> GetBlogsByFilterAsync(BlogFilterDto filter, Guid currentUserId);
        Task<IEnumerable<BlogResponseDto>> GetBlogsFirstRequestAsync(Guid currentUserId);
        Task<IEnumerable<Blog>> GetAllAsync();
        Task<List<Blog>> GetByIdsAsync(IEnumerable<int> ids);
    }

}
