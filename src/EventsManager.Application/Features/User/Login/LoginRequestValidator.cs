using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Features.User.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(e => e.Username).NotEmpty().WithMessage(SystemMessages.User.Error_Credentials);
            RuleFor(e => e.Password).NotEmpty().WithMessage(SystemMessages.User.Error_Credentials);
        }
    }
}

