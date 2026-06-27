using System.Text.Json.Serialization;

namespace EventsManager.Application.Features.Reservation.Confirm
{
    public class ConfirmReservationRequest
    {
        public int ReservationId { get; init; }
        [JsonIgnore]
        public string? ConfirmationCode { get; init; }
        public ConfirmReservationRequest(int reservationId, string? confirmationCode)
        {
            ReservationId = reservationId;
            ConfirmationCode = confirmationCode;
        }
        public static ConfirmReservationRequest From(int reservationId, string? confirmationCode)
        {
            return new(reservationId, confirmationCode);
        }
    }
}
