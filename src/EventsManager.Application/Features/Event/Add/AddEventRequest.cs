using EventsManager.Core.Enums;

namespace EventsManager.Application.Features.Event.Add
{
    public record AddEventRequest(
        string Title
        , string Description
        , int VenueId
        , int MaxCapacity
        , DateTime StartDate
        , DateTime EndDate
        , decimal Price
        , EventType EventType)
    {
        public static AddEventRequest From(string title, string description, int venueId, int maxCapacity, DateTime startDate, DateTime endDate, decimal price, EventType eventType)
        {
            return new(title, description, venueId, maxCapacity, startDate, endDate, price, eventType);
        }
    }
}
