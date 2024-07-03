using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class ChangeUsernameModel
{
    [Required(ErrorMessage = "New username is required")]
    public  string NewUsername { get; set; } = null!;
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}