using EventsManager.Application.Features.Venue.Add;
using EventsManager.Application.Features.Venue.Get;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Venue
{
    public record VenueUseCases(AddVenueHandler AddVenue, GetVenuesHandler GetVenues);
}
