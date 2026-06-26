using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Core.Entities;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Common.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Infrastructure.Persistence.Features.Event
{
    public class EventRepository : BaseRepository<EventEntity>, IEventRepository
    {
        public EventRepository(DbContextEventsManager dbContext) : base(dbContext)
        {
        }
    }
}
