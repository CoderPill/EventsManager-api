using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Features.Reservation.Add
{
    public class AddReservationHandler : BaseUseCase<AddReservationRequest, ReservationDTO>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IEventRepository _eventRepository;
        public AddReservationHandler(IValidator<AddReservationRequest> validator, IEventRepository eventRepository, IReservationRepository reservationRepository)
            : base(validator)
        {
            _eventRepository = eventRepository;
            _reservationRepository = reservationRepository;
        }
        protected override async Task<Result<ReservationDTO>> OnExecute(AddReservationRequest request)
        {
            return await ValidateReservationRules(request)
                        .BindAsync(AddReservation);
        }
        private async Task<Result<AddReservationRequest>> ValidateReservationRules(AddReservationRequest request)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity is null || !eventEntity.IsActive)
                return Result.Failure<AddReservationRequest>(string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Event));


            var now = DateTime.Now;
            var oneHour = 3600;
            var secondsUntilStart = (eventEntity.StartDate - now).TotalSeconds;
            if (secondsUntilStart < 0)
                return Result.Failure<AddReservationRequest>(SystemMessages.Validations.Rule_ReservationTooLate);
            if (secondsUntilStart < oneHour)
                return Result.Failure<AddReservationRequest>(SystemMessages.Validations.Rule_ReservationTooCloseToStart);


            var currentReservations = await _reservationRepository.GetCurrentOccupationByEventIdAsync(request.EventId);
            var availableCapacity = eventEntity.MaxCapacity - currentReservations;
            if (request.Quantity > availableCapacity)
                return Result.Failure<AddReservationRequest>(string.Format(SystemMessages.Validations.Rule_InsufficientCapacity, availableCapacity));


            if (secondsUntilStart < (oneHour * 24) && request.Quantity > 5)
                return Result.Failure<AddReservationRequest>(SystemMessages.Validations.Rule_MaxQuantityForLastDay);


            if (eventEntity.Price > 100 && request.Quantity > 10)
                return Result.Failure<AddReservationRequest>(SystemMessages.Validations.Rule_MaxQuantityForExpensiveEvent);


            return request;
        }
        private async Task<Result<ReservationDTO>> AddReservation(AddReservationRequest request)
        {
            var tempEntity = request.ToEntity();
            await _reservationRepository.AddAsync(tempEntity);
            await _reservationRepository.SaveChangesAsync();
            return tempEntity.ToDto();
        }
    }
}
