namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class UserQueryModel : QueryModel
{
    /// <summary>
    /// Search by username, first name, last name
    /// </summary>
    public new string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by Role Id
    /// Ex. 1,2,3 or 1, 2 or 1 
    /// </summary>
    public string? RoleIds { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsBanned { get; set; }

    /// <summary>
    /// Sort by id, username, first name, last name, role, is active, created at (default is id)
    /// </summary>
    public new string? SortColumn { get; set; }
}