using NET1814_MilkShop.Repositories.CoreHelpers.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET1814_MilkShop.Repositories.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Token is required!")]
        public string token { get; set; } = null!;
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
    }
}
