using GymSystem.DAL.Models.Enums;

namespace GymSystem.DAL.Models
{
    public class Trainer : GymUser
    {
        //Rename Created To HireDate
        public Speciality Speciality { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
