using EventsManager.Core.Entities;

namespace EventsManager.Application.Common.Interfaces.Persistence
{
    public interface IEventRepository : IBaseRepository<EventEntity>
    {
        Task<bool> HasOverlappingEventAsync(
        int venueId,
        DateTime startDate,
        DateTime endDate,
        int? excludeEventId = null);
    }
}
