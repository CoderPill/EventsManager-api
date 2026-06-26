using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Infrastructure.Persistence.Features.User;
using EventsManager.Infrastructure.Persistence.Features.Venue;
using EventsManager.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Infrastructure.Persistence.Common.DataSeed
{
    public static class MainSeeder
    {
        public static async Task SeedAllAsync(DbContext context, AdminSeedSettings adminSeedSettings, IPasswordHasher passwordHasher)
        {
            await UserSeeder.SeedAsync(context, adminSeedSettings, passwordHasher);
            await VenueSeeder.SeedAsync(context);

            await context.SaveChangesAsync();
        }
    }
}
