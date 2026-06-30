using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Common.Time;
using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Features.Reservation.Get
{
    public class GetReservationsHandler : BaseUseCase<Unit, List<ReservationDTO>>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IDateTimeProvider _timeProvider;
        public GetReservationsHandler(IValidator<Unit> validator, IReservationRepository reservationRepository, IDateTimeProvider timeProvider) : base(validator)
        {
            _reservationRepository = reservationRepository;
            _timeProvider = timeProvider;
        }
        protected override async Task<Result<List<ReservationDTO>>> OnExecute(Unit request)
        {
            var reservations = await _reservationRepository.GetAllAsync(null, true, SystemValues.QueryIncludes.Reservation_Event);
            return reservations.Select(r => r.ToDtoIncludeEvent(_timeProvider.GetNowColombia())).ToList();
        }
    }
}
