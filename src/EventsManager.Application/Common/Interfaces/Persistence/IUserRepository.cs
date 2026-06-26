using EventsManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository : IBaseRepository<UserEntity>
    {
        Task<UserEntity?> GetByUsernameAsync(string username);
    }
}
