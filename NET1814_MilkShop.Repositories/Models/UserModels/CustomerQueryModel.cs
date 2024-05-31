namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class CustomerQueryModel : QueryModel
{
    public bool? IsActive { get; set; }
    public bool? IsBanned { get; set; }
}
