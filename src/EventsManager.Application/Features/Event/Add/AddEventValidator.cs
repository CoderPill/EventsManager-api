using EventsManager.Core.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Event.Add
{
    public class AddEventValidator : AbstractValidator<AddEventRequest>
    {
        public AddEventValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.Name))
                .MinimumLength(5).WithMessage(string.Format(SystemMessages.Validations.Error_MinLength, SystemValues.PropertyNames.Title, SystemValues.Tags.Validator_MinLength))
                .MaximumLength(100).WithMessage(string.Format(SystemMessages.Validations.Error_MaxLength, SystemValues.PropertyNames.Title, SystemValues.Tags.Validator_MaxLength));

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.Description))
                .MinimumLength(10).WithMessage(string.Format(SystemMessages.Validations.Error_MinLength, SystemValues.PropertyNames.Description, SystemValues.Tags.Validator_MinLength))
                .MaximumLength(500).WithMessage(string.Format(SystemMessages.Validations.Error_MaxLength, SystemValues.PropertyNames.Description, SystemValues.Tags.Validator_MaxLength));

            RuleFor(x => x.VenueId)
                .GreaterThan(0).WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.Venue));

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0).WithMessage(string.Format(SystemMessages.Validations.Error_GreaterThan, SystemValues.PropertyNames.MaxCapacity, SystemValues.Tags.Validator_ComparisonValue));

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.StartDate))
                .GreaterThan(DateTime.Now).WithMessage(string.Format(SystemMessages.Validations.Error_GreaterThanOrEqual, SystemValues.PropertyNames.StartDate, SystemValues.PropertyNames.DateNow))
                .Must(BeValidWeekendStartTime).WithMessage(string.Format(SystemMessages.Validations.Rule_EventWeekendStartTimeLimit));

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.EndDate))
                .GreaterThan(x => x.StartDate).WithMessage(string.Format(SystemMessages.Validations.Error_GreaterThan, SystemValues.PropertyNames.EndDate, SystemValues.PropertyNames.StartDate));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage(string.Format(SystemMessages.Validations.Error_GreaterThan, SystemValues.PropertyNames.Price, SystemValues.Tags.Validator_ComparisonValue));

            RuleFor(x => x.EventType)
                .IsInEnum().WithMessage(string.Format(SystemMessages.Validations.Error_InvalidValue, SystemValues.PropertyNames.EventType));
        }

        private bool BeValidWeekendStartTime(DateTime startDate)
        {
            if (startDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return startDate.TimeOfDay <= new TimeSpan(22, 0, 0);

            return true;
        }
    }
}
