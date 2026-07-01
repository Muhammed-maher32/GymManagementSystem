using GymSystem.DAL.Models.Enums;

namespace GymSystem.BLL.ViewModels.MemberViewModels
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Photo { get; set; }
        public string Phone { get; set; } = default!;
        public Gender Gender { get; set; }
    }
}
