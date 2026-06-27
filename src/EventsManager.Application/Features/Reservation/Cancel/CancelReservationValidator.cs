using EventsManager.Core.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Reservation.Cancel
{
    public class CancelReservationValidator:AbstractValidator<CancelReservationRequest>
    {
        public CancelReservationValidator()
        {
            RuleFor(x => x.ReservationCode)
            .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.ReservationCode))
            .Matches(@"^EV-[A-Za-z0-9]{6}$").WithMessage(string.Format(SystemMessages.Validations.Error_InvalidFormat, SystemValues.PropertyNames.ReservationCode));
        }
    }
}
