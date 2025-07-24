using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(Guid userId, CreateCommentDTO dto);
        Task<List<CommentResponseDTO>> GetCommentsByBlogIdAsync(int blogId);
    }
}
