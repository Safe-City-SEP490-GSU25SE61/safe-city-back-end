using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IWardRepository : IGenericRepository<Ward>
    {
        Task<Ward> GetByNameAsync(string name); // Example: to fetch a ward by name
    }
}
