using EventsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventsManager.Infrastructure.Persistence.Features.Venue
{
    public static class VenueSeeder
    {
        public static async Task SeedAsync(DbContext context)
        {
            var hasData = await context.Set<VenueEntity>().AnyAsync();
            if (hasData) return;

            var defaultVenues = new List<VenueEntity>
        {
            new() { Name="Auditorio Central",Capacity=200,City="Bogota", CreateDate = DateTime.Now },
            new() { Name="Sala Norte",Capacity=50,City="Bogota", CreateDate = DateTime.Now },
            new() { Name="Sala Sur",Capacity=500,City="Medellin", CreateDate = DateTime.Now }
        };

            await context.Set<VenueEntity>().AddRangeAsync(defaultVenues);
        }
    }
}

