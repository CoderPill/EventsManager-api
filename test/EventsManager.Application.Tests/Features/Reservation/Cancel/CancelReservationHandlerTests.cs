using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Reservation.Cancel;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Constants;
using EventsManager.Core.Enums;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Features.Event;
using EventsManager.Infrastructure.Persistence.Features.Reservation;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.Reservation.Cancel;

public class CancelReservationHandlerTests : HandlerTestBase
{
    private sealed class TestState
    {
        public DbContextEventsManager DbContext { get; }
        public IEventRepository EventRepository { get; }
        public IReservationRepository ReservationRepository { get; }
        public FakeTimeProvider TimeProvider { get; } = new();
        public Mock<IValidator<CancelReservationRequest>> ValidatorMock { get; } = new();
        public CancelReservationHandler Handler { get; }
        public CancelReservationRequestBuilder RequestBuilder { get; } = new();

        public TestState()
        {
            DbContext = CreateInMemoryDbContext();
            EventRepository = new EventRepository(DbContext, TimeProvider);
            ReservationRepository = new ReservationRepository(DbContext, TimeProvider);

            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<CancelReservationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new CancelReservationHandler(
                ValidatorMock.Object,
                ReservationRepository,
                EventRepository,
                TimeProvider);
        }
    }

    [Fact]
    public async Task CancelReservation_WhenConfirmedReservation_ThenCancelsSuccessfully()
    {
        // Arrange
        var state = new TestState();

        // Seed Venue so Event FK resolves
        await SeedVenueAsync(state.DbContext, v => v.WithId(1));

        // Seed Event starting in 3 days (> 48h penalty window → no penalty)
        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .WithVenueId(1)
            .WithStartDate(state.TimeProvider.GetNowColombia().AddDays(3))
            .WithEndDate(state.TimeProvider.GetNowColombia().AddDays(3).AddHours(3)));

        // Seed a Confirmed reservation and detach to prevent tracking conflict
        // when the handler calls Update on a no-tracking query result
        var seededReservation = await SeedReservationAsync(state.DbContext, r => r
            .WithEventId(1)
            .WithId(1)
            .WithBuyerEmail("juan@test.com")
            .Confirmed()
            .WithReservationCode("EV-CANCEL-1"));
        state.DbContext.Entry(seededReservation).State = EntityState.Detached;

        var request = state.RequestBuilder
            .WithBuyerEmail("juan@test.com")
            .WithReservationCode("EV-CANCEL-1")
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify entity was updated in DB
        var cancelled = await state.ReservationRepository.GetByCodeAsync("juan@test.com", "EV-CANCEL-1");
        cancelled.Should().NotBeNull();
        cancelled!.Status.Should().Be(ReservationStatus.Cancelada);
        cancelled.CancelDate.Should().Be(state.TimeProvider.GetNowColombia());
        cancelled.HasPenalty.Should().BeFalse(); // 3 days > 48h penalty window
    }

    [Fact]
    public async Task CancelReservation_WhenWithin48Hours_ThenHasPenaltyIsTrue()
    {
        // Arrange
        var state = new TestState();

        await SeedVenueAsync(state.DbContext, v => v.WithId(1));

        // Seed Event starting in 24 hours (< 48h penalty window → penalty applies)
        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .WithVenueId(1)
            .WithStartDate(state.TimeProvider.GetNowColombia().AddHours(24))
            .WithEndDate(state.TimeProvider.GetNowColombia().AddHours(27)));

        var seededReservation = await SeedReservationAsync(state.DbContext, r => r
            .WithEventId(1)
            .WithId(1)
            .WithBuyerEmail("maria@test.com")
            .Confirmed()
            .WithReservationCode("EV-PENALTY-1"));
        state.DbContext.Entry(seededReservation).State = EntityState.Detached;

        var request = state.RequestBuilder
            .WithBuyerEmail("maria@test.com")
            .WithReservationCode("EV-PENALTY-1")
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var cancelled = await state.ReservationRepository.GetByCodeAsync("maria@test.com", "EV-PENALTY-1");
        cancelled.Should().NotBeNull();
        cancelled!.Status.Should().Be(ReservationStatus.Cancelada);
        cancelled.CancelDate.Should().Be(state.TimeProvider.GetNowColombia());
        cancelled.HasPenalty.Should().BeTrue(); // 24h < 48h penalty window
    }

    [Fact]
    public async Task CancelReservation_WhenNotFound_ThenReturnsFailureNotFound()
    {
        // Arrange
        var state = new TestState();
        var request = state.RequestBuilder
            .WithBuyerEmail("unknown@test.com")
            .WithReservationCode("NONEXISTENT")
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(
            string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Reservation));
    }

    [Fact]
    public async Task CancelReservation_WhenAlreadyCancelled_ThenReturnsFailureAlreadyCancelled()
    {
        // Arrange
        var state = new TestState();
        await SeedReservationAsync(state.DbContext, r => r
            .WithEventId(1)
            .WithId(1)
            .WithBuyerEmail("juan@test.com")
            .Cancelled()
            .WithReservationCode("EV-CANCEL-1"));

        var request = state.RequestBuilder
            .WithBuyerEmail("juan@test.com")
            .WithReservationCode("EV-CANCEL-1")
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_ReservationAlreadyCancelled);
    }

    [Fact]
    public async Task CancelReservation_WhenNotConfirmed_ThenReturnsFailureNotConfirmed()
    {
        // Arrange
        var state = new TestState();
        await SeedReservationAsync(state.DbContext, r => r
            .WithEventId(1)
            .WithId(1)
            .WithBuyerEmail("juan@test.com")
            .WithReservationCode("EV-CANCEL-1"));
        // Default status = PendientePago (not Confirmed)

        var request = state.RequestBuilder
            .WithBuyerEmail("juan@test.com")
            .WithReservationCode("EV-CANCEL-1")
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_ReservationNotConfirmed);
    }

    [Fact]
    public async Task CancelReservation_WhenEventNotFound_ThenReturnsFailureEventNotFound()
    {
        // Arrange
        var state = new TestState();
        await SeedReservationAsync(state.DbContext, r => r
            .WithEventId(1)
            .WithId(1)
            .WithBuyerEmail("juan@test.com")
            .Confirmed()
            .WithReservationCode("EV-CANCEL-1"));
        // No Event seeded — EventId=1 points to non-existent entity

        var request = state.RequestBuilder
            .WithBuyerEmail("juan@test.com")
            .WithReservationCode("EV-CANCEL-1")
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(
            string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Event));
    }

    [Fact]
    public async Task CancelReservation_WhenValidatorRejects_ThenReturnsFailure()
    {
        // Arrange
        const string expectedError = "La propiedad BuyerEmail es requerida";

        var state = new TestState();
        state.ValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CancelReservationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("BuyerEmail", expectedError)
            }));

        var request = state.RequestBuilder.Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(expectedError);
    }
}
