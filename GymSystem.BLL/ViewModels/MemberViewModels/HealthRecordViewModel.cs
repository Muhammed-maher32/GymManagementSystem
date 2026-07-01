using System.ComponentModel.DataAnnotations;

namespace GymSystem.BLL.ViewModels.MemberViewModels
{
    public class HealthRecordViewModel
    {
        [Range(0.1, 300, ErrorMessage = "Height must be greater than 0")]
        public int Height { get; set; }

        [Range(0.1, 500, ErrorMessage = "Weight must be greater than 0")]
        public float Weight { get; set; }

        [Required(ErrorMessage = "Blood Type Is Required")]
        [StringLength(3, ErrorMessage = "Blood type must be 3 characters or less")]
        public string BloodType { get; set; } = default!;
        public string? Notes { get; set; } = default!;
    }
}
