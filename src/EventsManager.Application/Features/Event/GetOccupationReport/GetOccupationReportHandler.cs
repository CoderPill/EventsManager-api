using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Features.Event.GetOccupationReport
{
    public class GetOccupationReportHandler : BaseUseCase<GetOccupationReportRequest, EventOccupationReportDto>
    {
        private readonly IEventRepository _eventRepository;

        public GetOccupationReportHandler(IValidator<GetOccupationReportRequest> validator, IEventRepository eventRepository)
            : base(validator)
        {
            _eventRepository = eventRepository;
        }

        protected override async Task<Result<EventOccupationReportDto>> OnExecute(GetOccupationReportRequest request)
        {
            var report = await _eventRepository.GetOccupationReportAsync(request.EventId);

            if (report is null)
                return Result.Failure<EventOccupationReportDto>(string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Event));

            return Result.Success(report);
        }
    }
}
