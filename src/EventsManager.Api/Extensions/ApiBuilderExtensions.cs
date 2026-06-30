using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Core.Common.Time;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Common.DataSeed;
using EventsManager.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;

namespace EventsManager.Api.Extensions
{
    public static class ApiBuilderExtensions
    {
        extension(IApplicationBuilder app)
        {
            public async Task ApplyMigrationsAndSeedAsync(AdminSeedSettings adminSeedConfig)
            {
                using var scope = app.ApplicationServices.CreateScope();
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<DbContextEventsManager>();
                    var passwordHasher = services.GetRequiredService<IPasswordHasher>();
                    var timeProvider = services.GetRequiredService<IDateTimeProvider>();

                    // 1. Migraciones Automáticas: Siempre se ejecutan al arrancar
                    if ((await context.Database.GetPendingMigrationsAsync()).Any())
                    {
                        if (context.Database.IsRelational())
                        {
                            await context.Database.MigrateAsync();
                        }

                        // 2. Seeding Manual: Se ejecuta SIEMPRE y cada seeder evalúa si su tabla está vacía
                        await MainSeeder.SeedAllAsync(context, adminSeedConfig, passwordHasher, timeProvider);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
