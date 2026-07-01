using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Event;
using EventsManager.Application.Features.Event.GetOccupationReport;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Constants;
using EventsManager.Core.Enums;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Features.Event;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.Event.GetOccupationReport;

public class GetOccupationReportHandlerTests : HandlerTestBase
{
    private sealed class TestState
    {
        public DbContextEventsManager DbContext { get; }
        public IEventRepository EventRepository { get; }
        public FakeTimeProvider TimeProvider { get; } = new();
        public Mock<IValidator<GetOccupationReportRequest>> ValidatorMock { get; } = new();
        public GetOccupationReportHandler Handler { get; }

        public TestState()
        {
            DbContext = CreateInMemoryDbContext();
            EventRepository = new EventRepository(DbContext, TimeProvider);

            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<GetOccupationReportRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new GetOccupationReportHandler(
                ValidatorMock.Object,
                EventRepository);
        }
    }

    [Fact]
    public async Task GetOccupationReport_WhenEventExists_ThenReturnsOccupationReportDto()
    {
        // Arrange
        var state = new TestState();
        var venue = await SeedVenueAsync(state.DbContext);
        var @event = await SeedEventAsync(state.DbContext, e => e
            .WithVenueId(venue.Id)
            .WithMaxCapacity(100)
            .WithTitle("Concert Test"));

        // Seed 2 confirmed reservations with different quantities and unique IDs
        await SeedReservationAsync(state.DbContext, r => r
            .WithId(1)
            .WithEventId(@event.Id)
            .WithQuantity(3)
            .Confirmed());

        await SeedReservationAsync(state.DbContext, r => r
            .WithId(2)
            .WithEventId(@event.Id)
            .WithQuantity(2)
            .Confirmed());

        var request = new GetOccupationReportRequest(@event.Id);

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.EventId.Should().Be(@event.Id);
        result.Value.Title.Should().Be("Concert Test");
        result.Value.TotalTicketsSold.Should().Be(5); // 3 + 2
        result.Value.TotalTicketsAvailable.Should().Be(95); // 100 - 5
        result.Value.OccupancyPercentage.Should().Be(5.0m); // (5/100) * 100
        result.Value.TotalRevenue.Should().Be(250m); // (3 + 2) * 50 (Event default price is 50)
    }

    [Fact]
    public async Task GetOccupationReport_WhenEventNotFound_ThenReturnsFailureNotFound()
    {
        // Arrange
        var state = new TestState();
        var request = new GetOccupationReportRequest(999);

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(
            string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Event));
    }
}
