using NET1814_MilkShop.Repositories.CoreHelpers.Validation;
using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.UserModels
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Old password is required")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        [StrongPassword]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
