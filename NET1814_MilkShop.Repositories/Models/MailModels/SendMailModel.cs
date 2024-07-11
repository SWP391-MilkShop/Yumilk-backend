namespace NET1814_MilkShop.Repositories.Models.MailModels;

public class SendMailModel
{
    public string? Receiver { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
}