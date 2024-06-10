using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using NET1814_MilkShop.Repositories.Models.MailModels;
using NET1814_MilkShop.Services.CoreHelpers;

namespace NET1814_MilkShop.Services.Services
{
    public interface IEmailService
    {
        void SendPasswordResetEmail(string receiveEmail, string token, string name);
        void SendVerificationEmail(string receiveEmail, string token,string name);
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
                IsBodyHtml = true,
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

        public void SendPasswordResetEmail( /*CustomerModel user*/
            string receiveEmail,
            string token,
            string name
        )
        {
                var model = new SendMailModel
                {
                    Receiver = receiveEmail,
                    Subject = "Đặt lại mật khẩu",
                    Body = MailBody.ResetPassword(name,token)
                };
                SendMail(model);
        }

        public void SendVerificationEmail( /*CustomerModel user*/
            string receiveEmail,
            string token,
            string name
        )
        {
                var model = new SendMailModel
                {
                    Receiver = receiveEmail,
                    Subject = "Kích hoạt tài khoản",
                    Body = MailBody.ActivateAccount(name, token)
                };
                SendMail(model);
        }
    }
}
