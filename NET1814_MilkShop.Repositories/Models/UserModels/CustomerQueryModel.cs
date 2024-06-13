namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class CustomerQueryModel : QueryModel
{
    public bool? IsActive { get; set; }
    public bool? IsBanned { get; set; }
    /// <summary>
    /// Sort by point, email, isactive, first name, last name, created at (default is id)
    /// </summary>
    public new string? SortColumn { get; set; }
}
