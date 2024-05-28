using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class ForgotPasswordModel
{
    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Must be email format!")]
    public string Email { get; set; } = null!;
}