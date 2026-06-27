using EventsManager.Application.Features.Reservation;
using EventsManager.Application.Features.Venue;
using EventsManager.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Event
{
    public record EventDTO(
        int Id
        ,string Title
        , string Description
        , int VenueId
        ,int MaxCapacity
        ,DateTime StartDate
        ,DateTime EndDate
        ,decimal Price
        ,EventType Type
        , bool IsActive
        ,DateTime CreationDate
        ,VenueDto? Venue = null
        ,ICollection<ReservationDTO>? Reservations = null)
    {
        public static EventDTO From(int id, string title, string description, int venueId, int maxCapacity, DateTime startDate, DateTime endDate, decimal price, EventType type, bool isActive,DateTime CreationDate, VenueDto? Venue = null, ICollection<ReservationDTO>? Reservations = null)
        {
            return new(id, title, description, venueId, maxCapacity, startDate, endDate, price, type, isActive, CreationDate, Venue, Reservations);
        }
    }
}
