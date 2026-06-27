namespace EventsManager.Application.Features.Venue
{
    public record VenueDto(
        int Id
        , string Name
        , int Capacity
        , string City
        , DateTime CreationDate)
    {
        public static VenueDto From(int id, string name, int capacity, string city, DateTime creationDate)
        {
            return new(id, name, capacity, city, creationDate);
        }
    }
}
