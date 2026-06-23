using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class Member : GymUser
    {
        //Rename CreatedAt To JionDate
        public string? Photo { get; set; }

        public HealthRecord HealthRecord { get; set; }
        public IEnumerable<Membership> Memberships { get; set; }
        public IEnumerable<Booking> Bookings { get; set; }

    }
}
