using Microsoft.Extensions.Options;
using NET1814_MilkShop.Repositories.Models;
using System.Net;
using System.Net.Mail;

namespace NET1814_MilkShop.Services.Services
{
    public interface IEmailService
    {
        void SendPasswordResetEmail(string receiveEmail, string token);
        void SendVerificationEmail(string receiveEmail, string token);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettingModel _emailSettingModel;

        public EmailService(IOptions<EmailSettingModel> options)
        {
            _emailSettingModel = options.Value;
        }

        public void SendMail(SendMailModel model)
        {
            var fromEmailAddress = _emailSettingModel.FromEmailAddress;
            var fromDisplayName = _emailSettingModel.FromDisplayName;
            if (fromEmailAddress == null)
            {
                throw new ArgumentException("FromEmailAddress is not set in EmailSettingModel");
            }
            MailMessage mailMessage = new MailMessage()
            {
                Subject = model.Subject,
                Body = model.Body,
                //có thể dùng html format để làm mail đẹp hơn
                IsBodyHtml = false,
            };
            mailMessage.From = new MailAddress(fromEmailAddress, fromDisplayName);
            mailMessage.To.Add(model.Receiver);

            var smtp = new SmtpClient()
            {
                EnableSsl = _emailSettingModel.Smtp.EnableSsl,
                Host = _emailSettingModel.Smtp.Host,
                Port = _emailSettingModel.Smtp.Port,
            };
            var network = new NetworkCredential(
                _emailSettingModel.Smtp.EmailAddress,
                _emailSettingModel.Smtp.Password
            );
            smtp.Credentials = network;
            //Send mail
            smtp.Send(mailMessage);
        }

        public void SendPasswordResetEmail(/*CustomerModel user*/ string receiveEmail, string token)
        {
            var model = new SendMailModel
            {
                Receiver = receiveEmail,
                Subject = "Password Reset",
                Body = "Please click the link below to reset your password\n\n" +
                $"https://localhost:5000/api/authentication/reset-password?token={token}"
            };
            SendMail(model);
        }

        public void SendVerificationEmail(/*CustomerModel user*/ string receiveEmail, string token)
        {
            var model = new SendMailModel
            {
                Receiver = receiveEmail,
                Subject = "Account Verification",
                Body = "Please click the link below to verify your account\n\n" +
                $"https://localhost:5000/api/authentication/verify?token={token}"
            };
            SendMail(model);
        }
    }
}
