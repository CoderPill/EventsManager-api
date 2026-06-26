using EventsManager.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventsManager.Infrastructure.Persistence.Common.Context
{
    public class DbContextEventsManagerFactory : IDesignTimeDbContextFactory<DbContextEventsManager>
    {
        public DbContextEventsManager CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable($"{SystemValues.Infrastructure.EnvironmentPrefix}{SystemValues.Infrastructure.DbContextFactoryConnectionStringEnvKey}");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException(string.Format(SystemMessages.Infrastructure.Error_InitConfiguration, typeof(DbContextEventsManagerFactory)));

            var optionsBuilder = new DbContextOptionsBuilder<DbContextEventsManager>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DbContextEventsManager(optionsBuilder.Options);
        }
    }
}
