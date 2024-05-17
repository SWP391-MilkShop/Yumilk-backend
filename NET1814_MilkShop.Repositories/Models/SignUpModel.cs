using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using NET1814_MilkShop.Repositories.CoreHelpers.Validation;

namespace NET1814_MilkShop.Repositories.Models
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "Username is required!")]
        [Display(Name = "Name")]
        public string Username { get; set; } = "";
        [Required(ErrorMessage = "First Name is required!")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required!")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Phone number is required!")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid Phone Number!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number!")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = "";
        [Required(ErrorMessage = "Email is required!"), EmailAddress(ErrorMessage = "Must be email format!")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StrongPassword]
        public string Password { get; set; } = "";
        [Required(ErrorMessage = "Confirm Password is required!")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation does not match!")]
        public string ConfirmPassword { get; set; } = "";
    }
}
