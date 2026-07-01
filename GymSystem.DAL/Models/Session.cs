namespace GymSystem.DAL.Models
{
    public class Session : BaseEntity
    {
        public string Description { get; set; } = default!;
        public int Capacity { get; set; }
        public DateTime EndDate { get; set; }
        //Rename Created At To Start Date

        public IEnumerable<Booking> Bookings { get; set; }
        public Trainer Trainer { get; set; }
        public int TrainerId { get; set; }

        public Category Category { get; set; }
        public int CategoryId { get; set; }

    }
}
