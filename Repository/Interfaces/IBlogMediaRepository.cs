using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IBlogMediaRepository
    {
        Task AddAsync(BlogMedia media);
        Task<List<string>> GetUrlsByPostIdAsync(int blogId);
    }
}
