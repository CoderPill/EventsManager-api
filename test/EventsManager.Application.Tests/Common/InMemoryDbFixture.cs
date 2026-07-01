using EventsManager.Infrastructure.Persistence.Common.Context;
using Microsoft.EntityFrameworkCore;

namespace EventsManager.Application.Tests.Common;

/// <summary>
/// xUnit <see cref="IClassFixture{T}"/> that provides fresh InMemory <see cref="DbContextEventsManager"/>
/// instances with unique database names (via <c>Guid.NewGuid()</c>) for parallel test safety.
/// </summary>
public class InMemoryDbFixture
{
    /// <summary>
    /// Creates a new <see cref="DbContextEventsManager"/> backed by an InMemory database
    /// with a unique name. Each call returns an isolated context.
    /// </summary>
    public DbContextEventsManager CreateContext()
    {
        var options = new DbContextOptionsBuilder<DbContextEventsManager>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DbContextEventsManager(options);
    }

    public void Dispose()
    {
        // No managed resources to dispose — InMemory database is GC-collected.
    }
}
