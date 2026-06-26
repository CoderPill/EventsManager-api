using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Core.Entities;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Common.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Infrastructure.Persistence.Features.Venue
{
    public class VenueRepository : BaseRepository<VenueEntity>, IVenueRepository
    {
        public VenueRepository(DbContextEventsManager dbContext) : base(dbContext)
        {
        }
    }
}
