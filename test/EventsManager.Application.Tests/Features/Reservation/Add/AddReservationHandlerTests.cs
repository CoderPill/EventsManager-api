using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Reservation;
using EventsManager.Application.Features.Reservation.Add;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Constants;
using EventsManager.Core.Enums;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Features.Event;
using EventsManager.Infrastructure.Persistence.Features.Reservation;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.Reservation.Add;

public class AddReservationHandlerTests : HandlerTestBase
{
    private sealed class TestState
    {
        public DbContextEventsManager DbContext { get; }
        public IEventRepository EventRepository { get; }
        public IReservationRepository ReservationRepository { get; }
        public FakeTimeProvider TimeProvider { get; } = new();
        public Mock<IValidator<AddReservationRequest>> ValidatorMock { get; } = new();
        public AddReservationHandler Handler { get; }
        public AddReservationRequestBuilder RequestBuilder { get; } = new();

        public TestState()
        {
            DbContext = CreateInMemoryDbContext();
            EventRepository = new EventRepository(DbContext, TimeProvider);
            ReservationRepository = new ReservationRepository(DbContext, TimeProvider);

            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<AddReservationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new AddReservationHandler(
                ValidatorMock.Object,
                EventRepository,
                ReservationRepository,
                TimeProvider);
        }
    }

    [Fact]
    public async Task AddReservation_WhenValidRequest_ThenReturnsSuccessWithReservationDto()
    {
        // Arrange
        var state = new TestState();
        await SeedEventAsync(state.DbContext, e => e.WithId(1));

        var request = state.RequestBuilder
            .WithEventId(1)
            .WithQuantity(2)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.EventId.Should().Be(1);
        result.Value.Quantity.Should().Be(2);
        result.Value.BuyerName.Should().Be("Juan Pérez");
        result.Value.BuyerEmail.Should().Be("juan@test.com");
        result.Value.Status.Should().Be(ReservationStatus.PendientePago);
    }

    [Fact]
    public async Task AddReservation_WhenEventNotFound_ThenReturnsFailureNotFound()
    {
        // Arrange
        var state = new TestState();
        var request = state.RequestBuilder
            .WithEventId(999)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(
            string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Event));
    }

    [Fact]
    public async Task AddReservation_WhenEventInactive_ThenReturnsFailureNotFound()
    {
        // Arrange
        var state = new TestState();
        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .Inactive());

        var request = state.RequestBuilder
            .WithEventId(1)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(
            string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Event));
    }

    [Fact]
    public async Task AddReservation_WhenEventAlreadyStarted_ThenReturnsFailureTooLate()
    {
        // Arrange
        var state = new TestState();
        var pastStart = state.TimeProvider.GetNowColombia().AddDays(-1);
        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .WithDates(pastStart, pastStart.AddHours(3)));

        var request = state.RequestBuilder
            .WithEventId(1)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_ReservationTooLate);
    }

    [Fact]
    public async Task AddReservation_WhenLessThan1Hour_ThenReturnsFailureTimeWindow()
    {
        // Arrange
        var state = new TestState();
        var nearStart = state.TimeProvider.GetNowColombia().AddMinutes(30);
        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .WithDates(nearStart, nearStart.AddHours(3)));

        var request = state.RequestBuilder
            .WithEventId(1)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_ReservationTooCloseToStart);
    }

    [Fact]
    public async Task AddReservation_WhenCapacityExceeds_ThenReturnsFailureCapacity()
    {
        // Arrange
        var state = new TestState();
        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .WithMaxCapacity(50));

        await SeedReservationAsync(state.DbContext, r => r
            .WithEventId(1)
            .WithQuantity(49)
            .Confirmed());

        var request = state.RequestBuilder
            .WithEventId(1)
            .WithQuantity(2)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(
            string.Format(SystemMessages.Validations.Rule_InsufficientCapacity, 1));
    }

    [Fact]
    public async Task AddReservation_WhenLessThan24hrsAndQtyExceeds5_ThenReturnsFailureQtyLimit()
    {
        // Arrange
        var state = new TestState();
        var soonStart = state.TimeProvider.GetNowColombia().AddHours(12);
        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .WithDates(soonStart, soonStart.AddHours(3))
            .WithMaxCapacity(100));

        var request = state.RequestBuilder
            .WithEventId(1)
            .WithQuantity(6)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_MaxQuantityForLastDay);
    }

    [Fact]
    public async Task AddReservation_WhenPriceExceeds100AndQtyExceeds10_ThenReturnsFailurePriceQty()
    {
        // Arrange
        var state = new TestState();
        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .WithPrice(150m)
            .WithMaxCapacity(100));

        var request = state.RequestBuilder
            .WithEventId(1)
            .WithQuantity(12)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_MaxQuantityForExpensiveEvent);
    }

    [Fact]
    public async Task AddReservation_WhenValidatorRejects_ThenReturnsFailure()
    {
        // Arrange
        const string expectedError = "La propiedad EventId es requerida";

        var state = new TestState();
        state.ValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<AddReservationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("EventId", expectedError)
            }));

        var request = state.RequestBuilder.Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(expectedError);
    }
}
