using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;

        public IEnumerable<Session> Sessions { get; set; }

    }
}
