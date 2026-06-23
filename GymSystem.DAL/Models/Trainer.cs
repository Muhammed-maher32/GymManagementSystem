using GymSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class Trainer : GymUser
    {
        //Rename Created To HireDate
        public Speciality Speciality { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
