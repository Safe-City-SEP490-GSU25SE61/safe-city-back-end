using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BusinessObject.Events
{
    public class PointChangedEvent : INotification
    {
        public Guid UserId { get; set; }
        public int NewTotalPoint { get; set; }
    }
}
