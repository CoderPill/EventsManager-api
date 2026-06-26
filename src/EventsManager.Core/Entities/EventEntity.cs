using EventsManager.Core.Enums;

namespace EventsManager.Core.Entities
{
    public class EventEntity : BaseEntity
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int VenueId { get; set; }

        public int MaxCapacity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public EventType Type { get; set; }

        public bool IsActive { get; set; }

        public virtual VenueEntity? Venue { get; set; }
        public virtual ICollection<ReservationEntity>? Reservations { get; set; }
    }
}
