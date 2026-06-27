namespace EventsManager.Application.Common.DTOs
{
    public record EmailMessage(string To, string Subject, string Body)
    {
        public static EmailMessage From(string to, string subject, string body)
        {
            return new(to, subject, body);
        }
    }
}
