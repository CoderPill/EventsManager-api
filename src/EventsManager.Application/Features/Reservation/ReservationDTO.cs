using EventsManager.Application.Features.Event;
using EventsManager.Core.Enums;

namespace EventsManager.Application.Features.Reservation
{
    public record ReservationDTO(
        int Id
        , int EventId
        , string BuyerName
        , string BuyerEmail
        , int Quantity
        , ReservationStatus Status
        , string? ReservationCode
        , DateTime? CancelDate
        , bool HasPenalty
        , DateTime CreationDate
        , EventDTO? Event = null)
    {
        public static ReservationDTO From(int id, int eventId, string buyerName, string buyerEmail, int quantity, ReservationStatus status, string? reservationCode, DateTime? cancelDate, bool hasPenalty, DateTime creationDate, EventDTO? _event = null)
        {
            return new(id, eventId, buyerName, buyerEmail, quantity, status, reservationCode, cancelDate, hasPenalty, creationDate, _event);
        }
    }

}
