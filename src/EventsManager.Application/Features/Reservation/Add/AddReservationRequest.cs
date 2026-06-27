namespace EventsManager.Application.Features.Reservation.Add
{
    public record AddReservationRequest(
        int EventId
        , int Quantity
        , string BuyerName
        , string BuyerEmail)
    {
        public static AddReservationRequest From(int eventId, int quantity, string buyerName, string buyerEmail)
        {
            return new(eventId, quantity, buyerName, buyerEmail);
        }
    }
}
