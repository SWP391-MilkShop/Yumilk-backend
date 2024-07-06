namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string receiveEmail, string token, string name);
    Task SendVerificationEmailAsync(string receiveEmail, string token, string name);
    Task SendPurchaseEmailAsync(string receiveEmail, string name);
    Task SendGoogleAccountAsync(string receiveEmail, string userFullName, string username, string password);
    Task SendActiveEmailAsync(string email, string userFullName);
}