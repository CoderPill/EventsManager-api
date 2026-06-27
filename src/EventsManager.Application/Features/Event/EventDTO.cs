using EventsManager.Application.Features.Reservation;
using EventsManager.Application.Features.Venue;
using EventsManager.Core.Enums;

namespace EventsManager.Application.Features.Event
{
    public record EventDTO(
        int Id
        , string Title
        , string Description
        , int VenueId
        , int MaxCapacity
        , DateTime StartDate
        , DateTime EndDate
        , decimal Price
        , EventType Type
        , EventStatus Status
        , DateTime CreationDate
        , VenueDto? Venue = null
        , ICollection<ReservationDTO>? Reservations = null)
    {
        public static EventDTO From(int id, string title, string description, int venueId, int maxCapacity, DateTime startDate, DateTime endDate, decimal price, EventType type, EventStatus status, DateTime CreationDate, VenueDto? Venue = null, ICollection<ReservationDTO>? Reservations = null)
        {
            return new(id, title, description, venueId, maxCapacity, startDate, endDate, price, type, status, CreationDate, Venue, Reservations);
        }
    }
    public record EventOccupationReportDto(
        int EventId
        , string Title
        , EventStatus Status
        , int TotalTicketsSold
        , int TotalTicketsAvailable
        , decimal OccupancyPercentage
        , decimal TotalRevenue
        , DateTime StartDate
        , DateTime EndDate)
    {
        public static EventOccupationReportDto From(int eventId, string title, EventStatus status, int totalTicketsSold, int totalTicketsAvailable, decimal occupancyPercentage, decimal totalRevenue, DateTime startDate, DateTime endDate)
        {
            return new(eventId, title, status, totalTicketsSold, totalTicketsAvailable, occupancyPercentage, totalRevenue, startDate, endDate);
        }

    }
}
