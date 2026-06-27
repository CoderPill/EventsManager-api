using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Core.Constants;
using EventsManager.Core.Entities;
using EventsManager.Core.Enums;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Common.Repository;
using Microsoft.EntityFrameworkCore;

namespace EventsManager.Infrastructure.Persistence.Features.Reservation
{
    public class ReservationRepository : BaseRepository<ReservationEntity>, IReservationRepository
    {
        public ReservationRepository(DbContextEventsManager dbContext) : base(dbContext)
        {
        }
        public async Task<int> GetCurrentOccupationByEventIdAsync(int eventId)
        {
            var query = BuildQuery(e=>e.EventId == eventId && e.Status == ReservationStatus.Confirmed || e.HasPenalty);
            return await query.SumAsync(r => r.Quantity);
        }
        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _DbSet.AnyAsync(r => r.ReservationCode == code);
        }
        public async Task<ReservationEntity?> GetByCodeAsync(string code)
        {
            return await GetFirstOrDefaultAsync(e => e.ReservationCode == code);
        }
        public async Task<ReservationEntity?> GetByCodeIncludeEventAsync(string buyerEmail,string code)
        {
            return await GetFirstOrDefaultAsync(e => e.ReservationCode == code && e.BuyerEmail.Equals(buyerEmail),true,SystemValues.QueryIncludes.Reservation_Event);
        }
    }
}
