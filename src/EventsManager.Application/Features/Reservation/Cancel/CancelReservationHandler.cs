using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Common.Time;
using EventsManager.Core.Constants;
using EventsManager.Core.Enums;
using FluentValidation;

namespace EventsManager.Application.Features.Reservation.Cancel
{
    public class CancelReservationHandler : BaseUseCase<CancelReservationRequest, Unit>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IDateTimeProvider _timeProvider;

        public CancelReservationHandler(IValidator<CancelReservationRequest> validator, IReservationRepository reservationRepository, IEventRepository eventRepository, IDateTimeProvider timeProvider)
            : base(validator)
        {
            _reservationRepository = reservationRepository;
            _eventRepository = eventRepository;
            _timeProvider = timeProvider;
        }

        protected override async Task<Result<Unit>> OnExecute(CancelReservationRequest request)
        {
            var reservation = await _reservationRepository.GetByCodeAsync(request.BuyerEmail, request.ReservationCode);
            if (reservation is null)
                return Result.Failure(string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Reservation));


            if (reservation.Status == ReservationStatus.Cancelled)
                return Result.Failure(SystemMessages.Validations.Rule_ReservationAlreadyCancelled);


            if (reservation.Status != ReservationStatus.Confirmed)
                return Result.Failure(SystemMessages.Validations.Rule_ReservationNotConfirmed);


            var eventEntity = await _eventRepository.GetByIdAsync(reservation.EventId);
            if (eventEntity is null)
                return Result.Failure(string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Event));

            var now = _timeProvider.GetNowColombia();
            var secondsUntilStart = (eventEntity.StartDate - now).TotalSeconds;

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancelDate = now;
            reservation.HasPenalty = secondsUntilStart < (SystemValues.ReservationRules.SecondsPerHour * SystemValues.ReservationRules.HoursBeforeStartForCancellationPenalty);

            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();
            return Result.Success();
        }
    }
}
