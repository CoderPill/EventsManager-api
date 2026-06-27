using EventsManager.Core.Entities;

namespace EventsManager.Application.Common.Interfaces.Persistence
{
    public interface IReservationRepository : IBaseRepository<ReservationEntity>
    {
        Task<int> GetCurrentOccupationByEventIdAsync(int eventId);
        Task<bool> ExistsByCodeAsync(string code);
        Task<ReservationEntity?> GetByCodeAsync(string code);
        Task<ReservationEntity?> GetByCodeIncludeEventAsync(string buyerEmail,string code);
    }
}
