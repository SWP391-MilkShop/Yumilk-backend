namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class CustomerQueryModel : QueryModel
{
    // SearchTerm se search firstName, lastName, email, phoneNumber
    public bool? IsActive { get; set; }
}
