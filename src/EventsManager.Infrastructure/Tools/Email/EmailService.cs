using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Infrastructure.Settings;
using System.Net;
using System.Net.Mail;

namespace EventsManager.Infrastructure.Tools.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public EmailService(SmtpSettings settings)
        {
            _settings = settings;
        }

        public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword),
                EnableSsl = _settings.EnableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };
            mail.To.Add(message.To);

            await client.SendMailAsync(mail, ct);
        }
    }
}
