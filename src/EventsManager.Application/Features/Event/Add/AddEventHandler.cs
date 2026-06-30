using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Common.Time;
using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Features.Event.Add
{
    public class AddEventHandler : BaseUseCase<AddEventRequest, EventDTO>
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IDateTimeProvider _timeProvider;
        public AddEventHandler(IValidator<AddEventRequest> validator, IEventRepository eventRepository, IVenueRepository venueRepository, IDateTimeProvider timeProvider)
            : base(validator)
        {
            _eventRepository = eventRepository;
            _venueRepository = venueRepository;
            _timeProvider = timeProvider;
        }
        protected override async Task<Result<EventDTO>> OnExecute(AddEventRequest request)
        {
            return await ValidateVenue(request)
                 .BindAsync(ValidateScheduleOverlap)
                 .BindAsync(AddEvent);
        }
        private async Task<Result<AddEventRequest>> ValidateVenue(AddEventRequest request)
        {
            var tempVenue = await _venueRepository.GetByIdAsync(request.VenueId);
            if (tempVenue == null)
                return Result.Failure<AddEventRequest>(string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Venue));
            if (request.MaxCapacity > tempVenue.Capacity)
                return Result.Failure<AddEventRequest>(SystemMessages.Validations.Rule_EventCapacityLimit);

            return request;
        }
        private async Task<Result<AddEventRequest>> ValidateScheduleOverlap(AddEventRequest request)
        {

            var hasOverlap = await _eventRepository.HasOverlappingEventAsync(
                venueId: request.VenueId,
                startDate: request.StartDate,
                endDate: request.EndDate,
                excludeEventId: null);

            if (hasOverlap)
                return Result.Failure<AddEventRequest>(SystemMessages.Validations.Rule_EventScheduleOverlap);

            return request;
        }
        private async Task<Result<EventDTO>> AddEvent(AddEventRequest request)
        {
            var newEvent = request.ToEntity();
            await _eventRepository.AddAsync(newEvent);
            await _eventRepository.SaveChangesAsync();
            return newEvent.ToDto(_timeProvider.GetNowColombia());
        }
    }
}
