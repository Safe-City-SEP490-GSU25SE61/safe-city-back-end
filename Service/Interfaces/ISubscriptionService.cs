using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ISubscriptionService
    {
        Task<bool> IsSubscribed(Guid userId);
    }
}
