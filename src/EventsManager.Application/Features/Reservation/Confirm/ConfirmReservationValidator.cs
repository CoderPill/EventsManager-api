using EventsManager.Core.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Reservation.Confirm
{
    public class ConfirmReservationValidator:AbstractValidator<ConfirmReservationRequest>
    {
        public ConfirmReservationValidator()
        {
            RuleFor(e=>e.ReservationId)
                .GreaterThan(0).WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.Reservation));
        }
    }
}
