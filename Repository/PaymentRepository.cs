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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        public PaymentRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(Payment payment)
        {
            await _context.Set<Payment>().AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetByOrderCodeAsync(string orderCode)
        {
            return await _context.Set<Payment>()
                .Include(p => p.PayosTransaction)
                .Include(p => p.Subscription).ThenInclude(s => s.Package)
                .FirstOrDefaultAsync(p => p.PayosTransaction.OrderCode == orderCode);
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Set<Payment>().Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Payments
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Package)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Subscription).ThenInclude(s => s.Package).Include(p => p.User)
                .ToListAsync();
        }

    }
}
