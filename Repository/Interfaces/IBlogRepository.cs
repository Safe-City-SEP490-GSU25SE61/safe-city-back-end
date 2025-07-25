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
        Task<IEnumerable<BlogResponseDto>> GetVisibleByCommuneAsync(int districtId, Guid currentUserId);
        Task<IEnumerable<BlogResponseDto>> GetCreatedBlogsByUserAsync(Guid userId);
        Task<IEnumerable<BlogResponseForOfficerDto>> GetBlogsForOfficerAsync(int communeId);
    }

}
