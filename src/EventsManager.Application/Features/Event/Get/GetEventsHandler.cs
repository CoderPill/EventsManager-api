using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using FluentValidation;

namespace EventsManager.Application.Features.Event.Get
{
    public class GetEventsHandler : BaseUseCase<Unit, List<EventDTO>>
    {
        private readonly IEventRepository _eventRepository;

        public GetEventsHandler(IValidator<Unit> validator, IEventRepository eventRepository)
            : base(validator)
        {
            _eventRepository = eventRepository;
        }

        protected override async Task<Result<List<EventDTO>>> OnExecute(Unit request)
        {
            var tempEntities = await _eventRepository.GetAllAsync();
            return tempEntities.Select(e => e.ToDto()).ToList();
        }
    }
}
