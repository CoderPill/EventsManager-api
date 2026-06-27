using EventsManager.Application.Features.Venue.Add;
using EventsManager.Application.Features.Venue.Get;

namespace EventsManager.Application.Features.Venue
{
    public record VenueUseCases(AddVenueHandler AddVenue, GetVenuesHandler GetVenues);
}
