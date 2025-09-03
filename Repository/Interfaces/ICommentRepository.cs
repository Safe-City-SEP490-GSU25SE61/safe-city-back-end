using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<List<CommentResponseDTO>> GetCommentsByBlogIdAsync(int blogId);
    }

}
