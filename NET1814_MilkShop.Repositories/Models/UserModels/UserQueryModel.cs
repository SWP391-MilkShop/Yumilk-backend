namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class UserQueryModel : QueryModel
{
    //SearchTerm tu QueryModel se duoc su dung de tim kiem theo UserName,FirstName,LastName
    public string? Role { get; set; }

    /// <summary>
    ///     Default is true
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    ///     Default is false
    /// </summary>
    public bool IsBanned { get; set; } = false;
}