using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Common.Time;
using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Features.Reservation.GetByReservationCode
{
    public class GetByReservationCodeHandler : BaseUseCase<GetByReservationCodeRequest, ReservationDTO>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IDateTimeProvider _timeProvider;
        public GetByReservationCodeHandler(IValidator<GetByReservationCodeRequest> validator, IReservationRepository reservationRepository, IDateTimeProvider timeProvider)
            : base(validator)
        {
            _reservationRepository = reservationRepository;
            _timeProvider = timeProvider;
        }
        protected override async Task<Result<ReservationDTO>> OnExecute(GetByReservationCodeRequest request)
        {
            var temReservation = await _reservationRepository.GetByCodeIncludeEventAsync(request.BuyerEmail, request.ReservationCode);
            return temReservation is not null ? temReservation.ToDtoIncludeEvent(_timeProvider.GetNowColombia()) : Result.Failure<ReservationDTO>(string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Reservation));
        }
    }
}

