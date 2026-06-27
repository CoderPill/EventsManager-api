using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Reservation.Add
{
    public class AddReservationValidator : AbstractValidator<AddReservationRequest>
    {
        public AddReservationValidator()
        {
            RuleFor(x => x.EventId)
           .GreaterThan(0).WithMessage(string.Format(SystemMessages.Validations.Error_Required,SystemValues.PropertyNames.Event));

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1).WithMessage(string.Format(SystemMessages.Validations.Error_GreaterThanOrEqual, SystemValues.PropertyNames.Quantity, SystemValues.Tags.Validator_ComparisonValue));

            RuleFor(x => x.BuyerName)
                .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.Name))
                .MaximumLength(32).WithMessage(string.Format(SystemMessages.Validations.Error_MaxLength, SystemValues.PropertyNames.Name, SystemValues.Tags.Validator_MaxLength));

            RuleFor(x => x.BuyerEmail)
                .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.Email))
                .MaximumLength(64).WithMessage(string.Format(SystemMessages.Validations.Error_MaxLength, SystemValues.PropertyNames.Email, SystemValues.Tags.Validator_MaxLength))
                .EmailAddress().WithMessage(string.Format(SystemMessages.Validations.Error_InvalidFormat, SystemValues.PropertyNames.Email));
        }
    }
}
