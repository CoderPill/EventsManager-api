namespace EventsManager.Application.Features.Venue.Add
{
    public record AddVenueRequest(string Name, int Capacity, string City)
    {
        public static AddVenueRequest From(string name, int capacity, string city)
        {
            return new(name, capacity, city);
        }
    }
}
