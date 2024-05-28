namespace NET1814_MilkShop.Repositories.Models.MailModels
{
    public class EmailSettingModel
    {
        public string? FromEmailAddress { get; set; }
        public string? FromDisplayName { get; set; }
        public Smtp Smtp { get; set; } = null!;
    }

    public class Smtp
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string EmailAddress { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool EnableSsl { get; set; }
        public bool UseCredential { get; set; }
    }
}
