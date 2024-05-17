using System.ComponentModel.DataAnnotations;
using NET1814_MilkShop.Repositories.CoreHelpers.Validation;

namespace NET1814_MilkShop.Repositories.Models
{
    public class CreateUserModel
    {
        [Required(ErrorMessage = "Username is required!")]
        [Display(Name = "Name")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "First Name is required!")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required!")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StrongPassword]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm Password is required!")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation does not match!")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Role is required!")]
        [Display(Name = "Role")]
        [Range(2, 3, ErrorMessage = "Choose role in range 1-3")]
        public int RoleId { get; set; }
    }
}
