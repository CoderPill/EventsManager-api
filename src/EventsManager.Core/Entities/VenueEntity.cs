namespace EventsManager.Core.Entities
{
    public class VenueEntity : BaseEntity
    {
        public string Name { get; set; } = null!;

        public int Capacity { get; set; }

        public string City { get; set; } = null!;
    }
}
