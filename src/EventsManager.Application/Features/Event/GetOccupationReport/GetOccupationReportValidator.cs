using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Features.Event.GetOccupationReport
{
    public class GetOccupationReportValidator : AbstractValidator<GetOccupationReportRequest>
    {
        public GetOccupationReportValidator()
        {
            RuleFor(x => x.EventId)
                .GreaterThan(0)
                .WithMessage(string.Format(SystemMessages.Validations.Error_Required, SystemValues.PropertyNames.Event));
        }
    }
}
