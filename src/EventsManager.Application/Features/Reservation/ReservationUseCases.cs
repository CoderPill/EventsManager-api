using EventsManager.Application.Features.Reservation.Add;
using EventsManager.Application.Features.Reservation.Cancel;
using EventsManager.Application.Features.Reservation.Confirm;
using EventsManager.Application.Features.Reservation.Get;
using EventsManager.Application.Features.Reservation.GetByReservationCode;

namespace EventsManager.Application.Features.Reservation
{
    public record ReservationUseCases(GetReservationsHandler GetReservations
                                    , AddReservationHandler AddReservation
                                    , ConfirmReservationHandler ConfirmReservation
                                    , GetByReservationCodeHandler GetByReservationCode
                                    , CancelReservationHandler CancelReservation);
}
