using EventsManager.Core.Entities;
using EventsManager.Infrastructure.Persistence.Common.Context;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventsManager.Application.Tests.Common;

/// <summary>
/// Base class for all handler tests. Provides mock validators, InMemory DbContext creation,
/// and seeding helpers that delegate to <see cref="EntityBuilders"/> for entity defaults.
/// </summary>
public abstract class HandlerTestBase
{
    /// <summary>
    /// Creates a mock <see cref="IValidator{T}"/> that always returns a valid <see cref="ValidationResult"/>.
    /// </summary>
    protected static Mock<IValidator<T>> MockValidValidator<T>() where T : class
    {
        var mock = new Mock<IValidator<T>>();
        mock.Setup(v => v.ValidateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        return mock;
    }

    /// <summary>
    /// Creates a fresh InMemory <see cref="DbContextEventsManager"/> with a unique database name
    /// (via <c>Guid.NewGuid()</c>) to ensure parallel test safety.
    /// </summary>
    protected static DbContextEventsManager CreateInMemoryDbContext()
    {
        return CreateInMemoryDbContext(Guid.NewGuid().ToString());
    }

    /// <summary>
    /// Creates an InMemory <see cref="DbContextEventsManager"/> with the specified database name.
    /// Use this with the same name to create a second context that reads the persisted store
    /// independently, proving that <c>SaveChangesAsync</c> actually committed the data.
    /// </summary>
    protected static DbContextEventsManager CreateInMemoryDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<DbContextEventsManager>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new DbContextEventsManager(options);
    }

    /// <summary>
    /// Seeds a <see cref="VenueEntity"/> using <see cref="VenueEntityBuilder"/> defaults.
    /// Pass an optional <paramref name="configure"/> action to customize the entity.
    /// </summary>
    protected static async Task<VenueEntity> SeedVenueAsync(DbContextEventsManager context, Action<VenueEntityBuilder>? configure = null)
    {
        var builder = new VenueEntityBuilder();
        configure?.Invoke(builder);
        var venue = builder.Build();
        context.Venues.Add(venue);
        await context.SaveChangesAsync();
        return venue;
    }

    /// <summary>
    /// Seeds an <see cref="EventEntity"/> using <see cref="EventEntityBuilder"/> defaults.
    /// Pass an optional <paramref name="configure"/> action to customize the entity.
    /// </summary>
    protected static async Task<EventEntity> SeedEventAsync(DbContextEventsManager context, Action<EventEntityBuilder>? configure = null)
    {
        var builder = new EventEntityBuilder();
        configure?.Invoke(builder);
        var @event = builder.Build();
        context.Events.Add(@event);
        await context.SaveChangesAsync();
        return @event;
    }

    /// <summary>
    /// Seeds a <see cref="ReservationEntity"/> using <see cref="ReservationEntityBuilder"/> defaults.
    /// Pass an optional <paramref name="configure"/> action to customize the entity.
    /// </summary>
    protected static async Task<ReservationEntity> SeedReservationAsync(DbContextEventsManager context, Action<ReservationEntityBuilder>? configure = null)
    {
        var builder = new ReservationEntityBuilder();
        configure?.Invoke(builder);
        var reservation = builder.Build();
        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();
        return reservation;
    }

    /// <summary>
    /// Seeds a <see cref="UserEntity"/> using <see cref="UserEntityBuilder"/> defaults.
    /// Pass an optional <paramref name="configure"/> action to customize the entity.
    /// </summary>
    protected static async Task<UserEntity> SeedUserAsync(DbContextEventsManager context, Action<UserEntityBuilder>? configure = null)
    {
        var builder = new UserEntityBuilder();
        configure?.Invoke(builder);
        var user = builder.Build();
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}
