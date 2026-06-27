using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Core.Entities;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Common.Repository;
using Microsoft.EntityFrameworkCore;

namespace EventsManager.Infrastructure.Persistence.Features.Event
{
    public class EventRepository : BaseRepository<EventEntity>, IEventRepository
    {
        public EventRepository(DbContextEventsManager dbContext) : base(dbContext)
        {
        }

        public async Task<bool> HasOverlappingEventAsync(
            int venueId
            , DateTime startDate
            , DateTime endDate
            , int? excludeEventId = null)
        {
            var query = BuildQuery(e => e.VenueId == venueId && e.IsActive);

            // Excluir evento específico (útil para actualizaciones)
            if (excludeEventId.HasValue)
                query = query.Where(e => e.Id != excludeEventId.Value);

            // Lógica de superposición: A_start < B_end AND A_end > B_start
            return await query.AnyAsync(e =>e.StartDate < endDate && e.EndDate > startDate);
        }

    }

}
