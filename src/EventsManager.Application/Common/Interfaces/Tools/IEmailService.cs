using EventsManager.Application.Common.DTOs;

namespace EventsManager.Application.Common.Interfaces.Tools
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message, CancellationToken ct = default);
    }
}
