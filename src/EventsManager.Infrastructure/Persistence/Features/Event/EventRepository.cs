using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Features.Event;
using EventsManager.Core.Common.Time;
using EventsManager.Core.Constants;
using EventsManager.Core.Entities;
using EventsManager.Core.Enums;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Common.Repository;
using Microsoft.EntityFrameworkCore;

namespace EventsManager.Infrastructure.Persistence.Features.Event
{
    public class EventRepository : BaseRepository<EventEntity>, IEventRepository
    {
        public EventRepository(DbContextEventsManager dbContext, IDateTimeProvider timeProvider) : base(dbContext,timeProvider)
        {
            
        }

        public async Task<bool> HasOverlappingEventAsync(
            int venueId
            , DateTime startDate
            , DateTime endDate
            , int? excludeEventId = null)
        {
            var query = BuildQuery(e => e.VenueId == venueId && e.IsActive);

            if (excludeEventId.HasValue)
                query = query.Where(e => e.Id != excludeEventId.Value);

            return await query.AnyAsync(e => e.StartDate < endDate && e.EndDate > startDate);
        }
        public async Task<EventOccupationReportDto?> GetOccupationReportAsync(int eventId)
        {
            var colombiaNow = _timeProvider.GetNowColombia();
            return await BuildQuery(e => e.Id == eventId, true, SystemValues.QueryIncludes.Event_Reservations)
                .Select(e => new EventOccupationReportDto(
                    EventId: e.Id,
                    Title: e.Title,
                    Status: !e.IsActive ? EventStatus.Cancelado :
                            e.EndDate < colombiaNow ? EventStatus.Completado : EventStatus.Activo,
                    TotalTicketsSold: e.Reservations
                        .Where(r => r.Status == ReservationStatus.Confirmada)
                        .Sum(r => r.Quantity),
                    TotalTicketsAvailable: e.MaxCapacity -
                        e.Reservations.Where(r => r.Status == ReservationStatus.Confirmada)
                            .Sum(r => r.Quantity),
                    OccupancyPercentage: e.MaxCapacity > 0 ?
                        Math.Round((decimal)e.Reservations
                            .Where(r => r.Status == ReservationStatus.Confirmada)
                            .Sum(r => r.Quantity) / e.MaxCapacity * 100, 2) : 0m,
                    TotalRevenue: e.Reservations
                        .Where(r => r.Status == ReservationStatus.Confirmada)
                        .Sum(r => r.Quantity) * e.Price,
                    StartDate: e.StartDate,
                    EndDate: e.EndDate))
                .FirstOrDefaultAsync();
        }
    }

}
