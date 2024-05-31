using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.UserModels
{
    public class UpdateUserModel
    {
        [Required(ErrorMessage = "IsBanned is required")]
        public bool IsBanned { get; set; }
    }
}
