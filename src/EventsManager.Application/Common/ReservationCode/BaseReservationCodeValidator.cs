using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Common.ReservationCode
{
    public abstract class BaseReservationCodeValidator<T> : AbstractValidator<T>
    where T : ReservationCodeRequest
    {
        protected BaseReservationCodeValidator()
        {
            RuleFor(x => x.BuyerEmail)
             .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.Email))
             .MaximumLength(64).WithMessage(string.Format(SystemMessages.Validations.Error_MaxLength, SystemValues.PropertyNames.Email, SystemValues.Tags.Validator_MaxLength))
             .EmailAddress().WithMessage(string.Format(SystemMessages.Validations.Error_InvalidFormat, SystemValues.PropertyNames.Email));

            RuleFor(x => x.ReservationCode)
            .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.ReservationCode))
            .Matches(@"^EV-[A-Za-z0-9]{6}$").WithMessage(string.Format(SystemMessages.Validations.Error_InvalidFormat, SystemValues.PropertyNames.ReservationCode));
        }
    }
}
