namespace NET1814_MilkShop.Repositories.Models.UserModels
{
    public class UserModel
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Role { get; set; }

        public bool? IsActive { get; set; }
        public bool IsBanned { get; set; }
    }
}
