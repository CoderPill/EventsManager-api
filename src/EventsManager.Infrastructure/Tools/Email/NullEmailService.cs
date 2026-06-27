using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Tools;

namespace EventsManager.Infrastructure.Tools.Email
{
    public class NullEmailService : IEmailService
    {
        public Task SendAsync(EmailMessage message, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
