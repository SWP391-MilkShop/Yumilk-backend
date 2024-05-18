using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models
{
    public class RequestLoginModel
    {
        [Required(ErrorMessage = "UserName là bắt buộc")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc")]
        public string Password { get; set; }
    }
}
