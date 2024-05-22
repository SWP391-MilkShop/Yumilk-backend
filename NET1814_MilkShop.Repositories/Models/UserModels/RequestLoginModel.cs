using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.UserModels
{
    public class RequestLoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
