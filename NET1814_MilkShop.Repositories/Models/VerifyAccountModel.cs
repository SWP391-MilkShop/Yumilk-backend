using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models
{
    public class VerifyAccountModel
    {
        [Required(ErrorMessage = "Token is required!")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        [Display(Name = "Email")]
        public string email { get; set; } = null!;
        [Required(ErrorMessage = "Token is required!")]
        [Display(Name = "Token")]
        public string token { get; set; } = null!;
    }
}
