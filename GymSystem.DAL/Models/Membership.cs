namespace GymSystem.DAL.Models
{
    public class Membership : BaseEntity
    {
        public IEnumerable<Plan> Plans { get; set; } = [];
        public int PlanId { get; set; }
        public IEnumerable<Member> Members { get; set; } = [];
        public int MemberId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
