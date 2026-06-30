namespace EventsManager.Infrastructure.Settings
{
    public class SmtpSettings
    {
        public bool SendEnabled { get; set; }
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}
