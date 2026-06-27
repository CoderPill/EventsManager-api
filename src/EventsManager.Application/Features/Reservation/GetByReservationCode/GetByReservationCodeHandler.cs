using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Constants;
using FluentValidation;

namespace EventsManager.Application.Features.Reservation.GetByReservationCode
{
    public class GetByReservationCodeHandler : BaseUseCase<GetByReservationCodeRequest, ReservationDTO>
    {
        private readonly IReservationRepository _reservationRepository;
        public GetByReservationCodeHandler(IValidator<GetByReservationCodeRequest> validator, IReservationRepository reservationRepository)
            : base(validator)
        {
            _reservationRepository = reservationRepository;
        }
        protected override async Task<Result<ReservationDTO>> OnExecute(GetByReservationCodeRequest request)
        {
            var temReservation = await _reservationRepository.GetByCodeIncludeEventAsync(request.BuyerEmail, request.ReservationCode);
            return temReservation is not null ? temReservation.ToDtoIncludeEvent() : Result.Failure<ReservationDTO>(string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Reservation));
        }
    }
}

