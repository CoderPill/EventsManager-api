using EventsManager.Core.Enums;

namespace EventsManager.Core.Entities
{
    public class ReservationEntity : BaseEntity
    {
        public int EventId { get; set; }

        public string BuyerName { get; set; } = null!;

        public string BuyerEmail { get; set; } = null!;

        public int Quantity { get; set; }

        public ReservationStatus Status { get; set; }

        public string? ReservationCode { get; set; }

        public DateTime? CancelDate { get; set; }

        public bool HasPenalty { get; set; }
        public virtual EventEntity? Event { get; set; }
    }
}
