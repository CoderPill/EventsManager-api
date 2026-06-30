using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Common.Time;
using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Features.Event.Get
{
    public class GetEventsHandler : BaseUseCase<Unit, List<EventDTO>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IDateTimeProvider _timeProvider;

        public GetEventsHandler(IValidator<Unit> validator, IEventRepository eventRepository, IDateTimeProvider timeProvider)
            : base(validator)
        {
            _eventRepository = eventRepository;
            _timeProvider = timeProvider;
        }

        protected override async Task<Result<List<EventDTO>>> OnExecute(Unit request)
        {
            var tempEntities = await _eventRepository.GetAllAsync(null, true, SystemValues.QueryIncludes.Event_Venue);
            return tempEntities.Select(e => e.ToDtoIncludeVenue(_timeProvider.GetNowColombia())).ToList();
        }
    }
}
