using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace WebLayer.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration; // appsettings bağlantı
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // appsettings den gelen ayarlar
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];

            // e posta içeriği
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; // HTML formatı için

                // SMTP istemcisi oluştur
                using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true; // Güvenli bağlantı (SSL/TLS)
                    await smtpClient.SendMailAsync(mail);
                }
            }
        }
    }
}
