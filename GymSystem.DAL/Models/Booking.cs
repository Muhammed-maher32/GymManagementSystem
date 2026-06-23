using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class Booking : BaseEntity
    {
        public IEnumerable<Session> Sessions { get; set; } = [];
        public int SessionId { get; set; }
        public IEnumerable<Member> Members { get; set; } = [];
        public int MemberId { get; set; }

        public DateTime BookingDate { get; set; }
        public bool IsAttended { get; set; }
    }
}
