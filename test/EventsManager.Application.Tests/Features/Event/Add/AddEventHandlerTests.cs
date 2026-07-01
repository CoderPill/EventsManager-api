using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Event;
using EventsManager.Application.Features.Event.Add;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Constants;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Features.Event;
using EventsManager.Infrastructure.Persistence.Features.Venue;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.Event.Add;

public class AddEventHandlerTests : HandlerTestBase
{
    private sealed class TestState
    {
        public DbContextEventsManager DbContext { get; }
        public IEventRepository EventRepository { get; }
        public IVenueRepository VenueRepository { get; }
        public FakeTimeProvider TimeProvider { get; } = new();
        public Mock<IValidator<AddEventRequest>> ValidatorMock { get; } = new();
        public AddEventHandler Handler { get; }
        public AddEventRequestBuilder RequestBuilder { get; } = new();

        public TestState()
        {
            DbContext = CreateInMemoryDbContext();
            EventRepository = new EventRepository(DbContext, TimeProvider);
            VenueRepository = new VenueRepository(DbContext, TimeProvider);

            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<AddEventRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new AddEventHandler(
                ValidatorMock.Object,
                EventRepository,
                VenueRepository,
                TimeProvider);
        }
    }

    [Fact]
    public async Task AddEvent_WhenValidRequest_ThenReturnsSuccessWithEventDto()
    {
        // Arrange
        var state = new TestState();
        var venue = await SeedVenueAsync(state.DbContext);
        var request = state.RequestBuilder
            .WithVenueId(venue.Id)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be(request.Title);
        result.Value.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AddEvent_WhenVenueNotFound_ThenReturnsFailureNotFound()
    {
        // Arrange
        var state = new TestState();
        var request = state.RequestBuilder
            .WithVenueId(999)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(
            string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Venue));
    }

    [Fact]
    public async Task AddEvent_WhenCapacityExceedsVenue_ThenReturnsFailureCapacityExceeded()
    {
        // Arrange
        var state = new TestState();
        await SeedVenueAsync(state.DbContext, v => v.WithCapacity(50));
        var request = state.RequestBuilder
            .WithMaxCapacity(100)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_EventCapacityLimit);
    }

    [Fact]
    public async Task AddEvent_WhenOverlapsExistingEvent_ThenReturnsFailureOverlap()
    {
        // Arrange
        var state = new TestState();
        var venue = await SeedVenueAsync(state.DbContext);
        var colombiaNow = state.TimeProvider.GetNowColombia();
        var eventStart = colombiaNow.AddDays(2);
        var eventEnd = colombiaNow.AddDays(2).AddHours(3);

        await SeedEventAsync(state.DbContext, e => e
            .WithVenueId(venue.Id)
            .WithDates(eventStart, eventEnd));

        var request = state.RequestBuilder
            .WithVenueId(venue.Id)
            .WithDates(eventStart.AddHours(1), eventEnd.AddHours(1))
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_EventScheduleOverlap);
    }

    [Fact]
    public async Task AddEvent_WhenValidatorRejects_ThenReturnsFailure()
    {
        // Arrange
        const string expectedError = "La propiedad Title es requerida";

        var state = new TestState();
        state.ValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<AddEventRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Title", expectedError)
            }));

        var request = state.RequestBuilder.Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(expectedError);
    }
}
