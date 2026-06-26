using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Core.Entities;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Common.Repository;

namespace EventsManager.Infrastructure.Persistence.Features.Reservation
{
    public class ReservationRepository : BaseRepository<ReservationEntity>, IReservationRepository
    {
        public ReservationRepository(DbContextEventsManager dbContext) : base(dbContext)
        {
        }
    }
}
