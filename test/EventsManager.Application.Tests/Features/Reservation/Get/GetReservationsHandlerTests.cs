using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Reservation;
using EventsManager.Application.Features.Reservation.Get;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Enums;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Features.Reservation;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.Reservation.Get;

public class GetReservationsHandlerTests : HandlerTestBase
{
    private sealed class TestState
    {
        public DbContextEventsManager DbContext { get; }
        public IReservationRepository ReservationRepository { get; }
        public FakeTimeProvider TimeProvider { get; } = new();
        public Mock<IValidator<Unit>> ValidatorMock { get; } = new();
        public GetReservationsHandler Handler { get; }

        public TestState()
        {
            DbContext = CreateInMemoryDbContext();
            ReservationRepository = new ReservationRepository(DbContext, TimeProvider);

            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<Unit>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new GetReservationsHandler(
                ValidatorMock.Object,
                ReservationRepository,
                TimeProvider);
        }
    }

    [Fact]
    public async Task GetReservations_WhenReservationsExist_ThenReturnsListOfReservationDtos()
    {
        // Arrange
        var state = new TestState();

        // Seed a Venue so the Event FK resolves
        var venue = await SeedVenueAsync(state.DbContext, v => v.WithId(1).WithName("Main Venue"));

        // Seed an Event referencing the venue
        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .WithVenueId(venue.Id)
            .WithTitle("Rock Concert"));

        // Seed 2 reservations with different data
        await SeedReservationAsync(state.DbContext, r => r
            .WithId(1)
            .WithEventId(1)
            .WithBuyerEmail("juan@test.com")
            .WithQuantity(2)
            .Confirmed()
            .WithReservationCode("EV-001"));

        await SeedReservationAsync(state.DbContext, r => r
            .WithId(2)
            .WithEventId(1)
            .WithBuyerEmail("maria@test.com")
            .WithQuantity(5));

        // Act
        var result = await state.Handler.Execute(Unit.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        // First reservation: confirmed with code, Event navigation populated
        result.Value.Should().ContainSingle(r =>
            r.BuyerEmail == "juan@test.com" &&
            r.Quantity == 2 &&
            r.Status == ReservationStatus.Confirmada &&
            r.ReservationCode == "EV-001" &&
            r.Event != null &&
            r.Event.Title == "Rock Concert");

        // Second reservation: pending without code
        result.Value.Should().ContainSingle(r =>
            r.BuyerEmail == "maria@test.com" &&
            r.Quantity == 5 &&
            r.Status == ReservationStatus.PendientePago &&
            r.ReservationCode == null);
    }
}
