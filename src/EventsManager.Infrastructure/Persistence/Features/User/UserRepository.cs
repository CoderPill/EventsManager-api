using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Core.Entities;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Common.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Infrastructure.Persistence.Features.User
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(DbContextEventsManager dbContext)
            : base(dbContext)
        {
        }
        public async Task<UserEntity?> GetByUsernameAsync(string username)
        {
            return await GetFirstOrDefaultAsync(predicate: u => u.Username.Equals(username));
        }
    }
}
