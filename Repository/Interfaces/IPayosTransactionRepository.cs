using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IPayosTransactionRepository
    {
        Task AddAsync(PayosTransaction transaction);
        Task<PayosTransaction?> GetByOrderCodeAsync(string orderCode);
        Task UpdateAsync(PayosTransaction transaction);
    }

}
