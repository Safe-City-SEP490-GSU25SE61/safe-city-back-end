using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PayosTransactionRepository : IPayosTransactionRepository
    {
        private readonly AppDbContext _context;

        public PayosTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PayosTransaction transaction)
        {
            await _context.PayosTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<PayosTransaction?> GetByOrderCodeAsync(string orderCode)
        {
            return await _context.PayosTransactions
                .Include(p => p.Payment)
                .FirstOrDefaultAsync(p => p.OrderCode == orderCode);
        }

        public async Task UpdateAsync(PayosTransaction transaction)
        {
            _context.PayosTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
