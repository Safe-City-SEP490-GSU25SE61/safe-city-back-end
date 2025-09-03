using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IBlogLikeRepository
    {
        Task<bool> ExistsAsync(Guid userId, int blogId);
        Task<BlogLike?> GetAsync(Guid userId, int blogId);
        Task AddAsync(BlogLike like);
        Task RemoveAsync(BlogLike like);
    }
}
