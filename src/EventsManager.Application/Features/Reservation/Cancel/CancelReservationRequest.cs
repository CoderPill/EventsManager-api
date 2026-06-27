using EventsManager.Application.Common.ReservationCode;

namespace EventsManager.Application.Features.Reservation.Cancel
{
    public record CancelReservationRequest(string BuyerEmail, string ReservationCode)
        : ReservationCodeRequest(BuyerEmail, ReservationCode)
    {
        public static CancelReservationRequest From(string buyer, string reservationCode)
        {
            return new(buyer, reservationCode);
        }
    }
}
