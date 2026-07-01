using System.ComponentModel.DataAnnotations;

namespace GymSystem.BLL.ViewModels.MemberViewModels
{
    public class MemberToUpdateViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; } = default!;
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Phone Number Is Required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone number must be a valid Egyptian mobile number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = default!;
        public string? Photo { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int BuildingNumber { get; set; }


    }
}
