namespace GymSystem.DAL.Models
{
    public class HealthRecord : BaseEntity
    {
        public int Height { get; set; }
        public float Weight { get; set; }
        public string BloodType { get; set; } = default!;
        public string? Notes { get; set; } = default!;

        public Member Member { get; set; }
        public int MemberId { get; set; }
    }
}
