using EventsManager.Application.Common.ReservationCode;

namespace EventsManager.Application.Features.Reservation.GetByReservationCode
{
    public record GetByReservationCodeRequest(string BuyerEmail, string ReservationCode)
        : ReservationCodeRequest(BuyerEmail, ReservationCode)
    {
        public static GetByReservationCodeRequest From(string buyerEmail, string reservationCode)
        {
            return new(buyerEmail, reservationCode);
        }
    }
}
