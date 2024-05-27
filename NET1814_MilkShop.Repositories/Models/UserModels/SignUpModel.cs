using System.ComponentModel.DataAnnotations;
using NET1814_MilkShop.Repositories.CoreHelpers.Validation;

namespace NET1814_MilkShop.Repositories.Models.UserModels
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "First Name is required!")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required!")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required!")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid Phone Number!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number!")]
        public string PhoneNumber { get; set; } = null!;

        [
            Required(ErrorMessage = "Email is required!"),
            EmailAddress(ErrorMessage = "Must be email format!")
        ]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [StrongPassword]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm Password is required!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation does not match!")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
