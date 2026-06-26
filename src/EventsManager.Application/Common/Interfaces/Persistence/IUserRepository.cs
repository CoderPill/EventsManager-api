using EventsManager.Core.Entities;

namespace EventsManager.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository : IBaseRepository<UserEntity>
    {
        Task<UserEntity?> GetByUsernameAsync(string username);
    }
}
