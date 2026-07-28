using Microsoft.Extensions.Configuration;
using ShahrChap.Core.Services.Interfaces;
using System.Net.Mail;

namespace ShahrChap.Core.Services
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Send(string To,string Subject,string Body)
        {
            var email = _configuration["Email:Address"];
            var password = _configuration["Email:Password"];
            var displayName = _configuration["Email:DisplayName"];

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress(email,displayName);
            mail.To.Add(To);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential(email, password);
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
    }
}