namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class UserQueryModel : QueryModel
{
    //SearchTerm tu QueryModel se duoc su dung de tim kiem theo UserName,FirstName,LastName
    public string? Role { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsBanned { get; set; }
    /// <summary>
    /// Sort by id, username, first name, last name, role, is active, created at (default is id)
    /// </summary>
    public new string? SortColumn { get; set; }
}
