namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class CustomerQueryModel : QueryModel
{
    // SearchTerm se search firstName, lastName, email, phoneNumber
    /// <summary>
    /// Default is true
    /// </summary>
    public bool IsActive { get; set; } = true;
    /// <summary>
    /// Default is false
    /// </summary>
    public bool IsBanned { get; set; } = false;
}
