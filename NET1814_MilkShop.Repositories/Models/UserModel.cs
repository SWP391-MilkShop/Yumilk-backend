namespace NET1814_MilkShop.Repositories.Models
{
    public class UserModel
    {
        public string Username { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int? RoleId { get; set; }

        public bool? IsActive { get; set; }

    }
}
