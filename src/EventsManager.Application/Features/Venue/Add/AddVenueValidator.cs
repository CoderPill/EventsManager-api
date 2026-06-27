using EventsManager.Core.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Venue.Add
{
    public class AddVenueValidator : AbstractValidator<AddVenueRequest>
    {
        public AddVenueValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.Name))
            .MaximumLength(32).WithMessage(string.Format(SystemMessages.Validations.Error_MaxLength, SystemValues.PropertyNames.Name,SystemValues.Tags.Validator_MaxLength));

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage(string.Format(SystemMessages.Validations.Error_GreaterThan, SystemValues.PropertyNames.Capacity, SystemValues.Tags.Validator_ComparisonValue));

            RuleFor(x => x.City)
                .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.City))
                .MaximumLength(32).WithMessage(string.Format(SystemMessages.Validations.Error_MaxLength, SystemValues.PropertyNames.City, SystemValues.Tags.Validator_MaxLength));

        }
    }
}
