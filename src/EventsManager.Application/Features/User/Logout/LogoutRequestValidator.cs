using EventsManager.Core.Constants;
using FluentValidation;

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
