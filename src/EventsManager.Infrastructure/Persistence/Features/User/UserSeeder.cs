using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Core.Entities;
using EventsManager.Core.Enums;
using EventsManager.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Infrastructure.Persistence.Features.User
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(DbContext context, AdminSeedSettings adminSeedSettings, IPasswordHasher passwordHasher)
        {
            var hasUsers = await context.Set<UserEntity>().AnyAsync();
            if (hasUsers) return;

            var adminUsername = adminSeedSettings.Username;
            var adminPassword = adminSeedSettings.Password;

            string hashedPass = passwordHasher.Hash(adminPassword);

            var adminUser = new UserEntity
            {
                Username = adminUsername,
                PasswordHash = hashedPass,
                Role = UserRole.Admin,
                IsActive = true,
                CreateDate = DateTime.Now
            };

            await context.Set<UserEntity>().AddAsync(adminUser);
        }
    }
}
