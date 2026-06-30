using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Core.Common.Time;
using EventsManager.Core.Entities;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Common.Repository;

namespace EventsManager.Infrastructure.Persistence.Features.Venue
{
    public class VenueRepository : BaseRepository<VenueEntity>, IVenueRepository
    {
        public VenueRepository(DbContextEventsManager dbContext, IDateTimeProvider timeProvider) : base(dbContext, timeProvider)
        {
        }
    }
}
