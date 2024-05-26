using NET1814_MilkShop.Repositories.CoreHelpers.Validation;
using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.UserModels
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Token is required!")]
        public string token { get; set; } = null!;

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
