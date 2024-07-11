namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class ResponseLoginModel
{
    public string UserID { get; set; } = null!;
    public string Username { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? GoogleId { get; set; }
    public string? Role { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public bool IsBanned { get; set; }
    public bool IsActive { get; set; } = true;
}