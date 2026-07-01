using GymSystem.DAL.Models.Enums;

namespace GymSystem.BLL.ViewModels.MemberViewModels
{
    public class MemberDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Photo { get; set; }
        public string Phone { get; set; } = default!;
        public Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; }
        public string MembershipStartDate { get; set; }
        public string MembershipEndDate { get; set; }
        public string? PlanName { get; set; }


    }
}
