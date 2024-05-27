namespace NET1814_MilkShop.Repositories.Models
{
    public class ResponseModel
    {
        public string Status { get; set; } = null!;
        public string Message { get; set; } = null!;
        public object? Data { get; set; }
    }
}
