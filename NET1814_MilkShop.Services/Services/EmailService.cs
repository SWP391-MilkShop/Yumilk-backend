using NET1814_MilkShop.Repositories.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace NET1814_MilkShop.Services.Services
{
    public interface IEmailService
    {
        void SendPasswordResetEmail(CustomerModel user);
        void SendVerificationEmail(CustomerModel user);
    }
    public class EmailService : IEmailService
    {
        private readonly EmailSettingModel _emailSettingModel;
        public EmailService(IOptions<EmailSettingModel> options)
        {
            _emailSettingModel = options.Value;
        }
        private void SendMail(SendMailModel model)
        {
            var fromEmailAddress = _emailSettingModel.FromEmailAddress;
            var fromDisplayName = _emailSettingModel.FromDisplayName;
            if (fromEmailAddress == null)
            {
                throw new ArgumentException("FromEmailAddress is not set in EmailSettingModel");
            }
            MailMessage mailMessage = new MailMessage()
            {
                Subject = "",
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
            var network = new NetworkCredential(_emailSettingModel.Smtp.EmailAddress, _emailSettingModel.Smtp.Password);
            smtp.Credentials = network;
            //Send mail
            smtp.Send(mailMessage);
        }
        public void SendPasswordResetEmail(CustomerModel user)
        {
            var model = new SendMailModel
            {
                Receiver = user.Email,
                Subject = "Password Reset", 
                Body = "Please click the link below to reset your password" //nay chua co link nha ae
            };
            SendMail(model);
        }

        public void SendVerificationEmail(CustomerModel user)
        {
            var model = new SendMailModel
            {
                Receiver = user.Email,
                Subject = "Verification",
                Body = "Please click the link below to verify your account"
            };
            SendMail(model);
        }
    }
}
