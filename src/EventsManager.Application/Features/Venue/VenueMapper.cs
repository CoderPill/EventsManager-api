using EventsManager.Application.Features.Venue.Add;
using EventsManager.Core.Entities;

namespace EventsManager.Application.Features.Venue
{
    public static class VenueMapper
    {
        extension(VenueEntity instance)
        {
            public VenueDto ToDto()
            {
                return VenueDto.From(instance.Id, instance.Name, instance.Capacity, instance.City, instance.CreationDate);
            }
        }
        extension(AddVenueRequest instance)
        {
            public VenueEntity ToVenueEntity()
            {
                return new() { Name = instance.Name, Capacity = instance.Capacity, City = instance.City };
            }
        }

    }
}
