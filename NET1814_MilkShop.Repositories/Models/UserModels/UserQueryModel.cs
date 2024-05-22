namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class UserQueryModel : QueryModel
{
    //SearchTerm tu QueryModel se duoc su dung de tim kiem theo UserName,FirstName,LastName
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
