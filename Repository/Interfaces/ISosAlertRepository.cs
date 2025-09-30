using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ISosAlertRepository
    {
        Task<string> CreateAsync(SosAlert alert);
        Task<SosAlert?> GetByIdAsync(int id);
        Task UpdateAsync(SosAlert entity);
        Task<SosAlert?> GetLatestBySenderIdAsync(Guid senderId);
    }
}

