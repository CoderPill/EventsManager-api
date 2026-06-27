using EventsManager.Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace EventsManager.Application.Common.Interfaces.Tools
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message, CancellationToken ct = default);
    }
}
