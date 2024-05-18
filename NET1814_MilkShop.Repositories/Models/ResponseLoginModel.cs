namespace NET1814_MilkShop.Repositories.Models
{
    public class ResponseLoginModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
