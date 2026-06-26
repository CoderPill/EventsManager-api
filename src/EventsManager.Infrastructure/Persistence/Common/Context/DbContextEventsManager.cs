using EventsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventsManager.Infrastructure.Persistence.Common.Context
{
    public class DbContextEventsManager : DbContext
    {
        public DbContextEventsManager(DbContextOptions<DbContextEventsManager> options) : base(options)
        {
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<ReservationEntity> Reservations { get; set; }
        public DbSet<VenueEntity> Venues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextEventsManager).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
