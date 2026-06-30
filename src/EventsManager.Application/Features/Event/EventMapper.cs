using EventsManager.Application.Features.Event.Add;
using EventsManager.Application.Features.Reservation;
using EventsManager.Application.Features.Venue;
using EventsManager.Core.Entities;
using EventsManager.Core.Enums;

namespace EventsManager.Application.Features.Event
{
    public static class EventMapper
    {
        extension(EventEntity instance)
        {
            private EventStatus CalculateStatus(DateTime colombiaNow)
            {
                if (!instance.IsActive)
                    return EventStatus.Cancelado;

                if (instance.EndDate < colombiaNow)
                    return EventStatus.Completado;

                return EventStatus.Activo;
            }
            public EventDTO ToDto(DateTime colombiaNow)
            {
                return EventDTO.From(instance.Id, instance.Title, instance.Description, instance.VenueId, instance.MaxCapacity, instance.StartDate, instance.EndDate, instance.Price, instance.Type, instance.CalculateStatus(colombiaNow), instance.CreationDate);
            }
            public EventDTO ToDtoIncludeVenue(DateTime colombiaNow)
            {
                return EventDTO.From(instance.Id, instance.Title, instance.Description, instance.VenueId, instance.MaxCapacity, instance.StartDate, instance.EndDate, instance.Price, instance.Type, instance.CalculateStatus(colombiaNow), instance.CreationDate, instance.Venue?.ToDto());
            }
            public EventDTO ToDtoIncludeReservations(DateTime colombiaNow)
            {
                return EventDTO.From(instance.Id, instance.Title, instance.Description, instance.VenueId, instance.MaxCapacity, instance.StartDate, instance.EndDate, instance.Price, instance.Type, instance.CalculateStatus(colombiaNow), instance.CreationDate, null, instance.Reservations?.Select(r => r.ToDto()).ToList());
            }
            public EventDTO ToDtoIncludeVenueAndReservations(DateTime colombiaNow)
            {
                return EventDTO.From(instance.Id, instance.Title, instance.Description, instance.VenueId, instance.MaxCapacity, instance.StartDate, instance.EndDate, instance.Price, instance.Type, instance.CalculateStatus(colombiaNow), instance.CreationDate, instance.Venue?.ToDto(), instance.Reservations?.Select(r => r.ToDto()).ToList());
            }
        }

        extension(AddEventRequest instance)
        {
            public EventEntity ToEntity()
            {
                return new()
                {
                    Title = instance.Title
                ,
                    Description = instance.Description
                ,
                    VenueId = instance.VenueId
                ,
                    MaxCapacity = instance.MaxCapacity
                ,
                    StartDate = instance.StartDate
                ,
                    EndDate = instance.EndDate
                ,
                    Price = instance.Price
                ,
                    Type = instance.EventType
                ,
                    IsActive = true
                };
            }
        }
    }
}
