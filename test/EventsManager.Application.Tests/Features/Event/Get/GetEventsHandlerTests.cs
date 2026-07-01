using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Event;
using EventsManager.Application.Features.Event.Get;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Entities;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Features.Event;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.Event.Get;

public class GetEventsHandlerTests : HandlerTestBase
{
    private sealed class TestState
    {
        public DbContextEventsManager DbContext { get; }
        public IEventRepository EventRepository { get; }
        public FakeTimeProvider TimeProvider { get; } = new();
        public Mock<IValidator<Unit>> ValidatorMock { get; } = new();
        public GetEventsHandler Handler { get; }

        public TestState()
        {
            DbContext = CreateInMemoryDbContext();
            EventRepository = new EventRepository(DbContext, TimeProvider);

            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<Unit>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new GetEventsHandler(
                ValidatorMock.Object,
                EventRepository,
                TimeProvider);
        }
    }

    [Fact]
    public async Task GetEvents_WhenEventsExist_ThenReturnsListOfEventDtos()
    {
        // Arrange
        var state = new TestState();

        // Seed a Venue so that Include("Venue") navigation resolves in InMemory
        var venue = await SeedVenueAsync(state.DbContext, v => v.WithId(1).WithName("Test Venue"));

        // Seed events referencing the existing VenueId
        await SeedEventAsync(state.DbContext, e => e.WithId(1).WithVenueId(venue.Id).WithTitle("Event 1"));
        await SeedEventAsync(state.DbContext, e => e.WithId(2).WithVenueId(venue.Id).WithTitle("Event 2"));
        await SeedEventAsync(state.DbContext, e => e.WithId(3).WithVenueId(venue.Id).WithTitle("Event 3"));

        // Act
        var result = await state.Handler.Execute(Unit.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(e => e.Title == "Event 1");
        result.Value.Should().Contain(e => e.Title == "Event 2");
        result.Value.Should().Contain(e => e.Title == "Event 3");
    }
}
