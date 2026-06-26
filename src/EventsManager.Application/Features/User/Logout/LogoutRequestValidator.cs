using EventsManager.Application.Common.DTOs;
using EventsManager.Core.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.User.Logout
{
    public class LogoutRequestValidator : AbstractValidator<LogoutRequest>
    {
        public LogoutRequestValidator()
        {
            RuleFor(e => e.Jti)
                .NotEmpty().WithMessage(SystemMessages.User.Error_InvalidToken);

            RuleFor(e => e.ExpiresDate)
                .NotEmpty().WithMessage(SystemMessages.User.Error_InvalidToken);
        }
    }
}
