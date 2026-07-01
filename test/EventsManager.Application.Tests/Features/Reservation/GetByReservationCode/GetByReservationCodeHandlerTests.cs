using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Reservation;
using EventsManager.Application.Features.Reservation.GetByReservationCode;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Constants;
using EventsManager.Core.Enums;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Features.Reservation;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.Reservation.GetByReservationCode;

public class GetByReservationCodeHandlerTests : HandlerTestBase
{
    private sealed class TestState
    {
        public DbContextEventsManager DbContext { get; }
        public IReservationRepository ReservationRepository { get; }
        public FakeTimeProvider TimeProvider { get; } = new();
        public Mock<IValidator<GetByReservationCodeRequest>> ValidatorMock { get; } = new();
        public GetByReservationCodeHandler Handler { get; }

        public TestState()
        {
            DbContext = CreateInMemoryDbContext();
            ReservationRepository = new ReservationRepository(DbContext, TimeProvider);

            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<GetByReservationCodeRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new GetByReservationCodeHandler(
                ValidatorMock.Object,
                ReservationRepository,
                TimeProvider);
        }
    }

    [Fact]
    public async Task GetByReservationCode_WhenReservationExists_ThenReturnsReservationDtoWithEvent()
    {
        // Arrange
        var state = new TestState();

        var venue = await SeedVenueAsync(state.DbContext, v => v.WithId(1));

        await SeedEventAsync(state.DbContext, e => e
            .WithId(1)
            .WithVenueId(venue.Id)
            .WithTitle("Rock Concert"));

        await SeedReservationAsync(state.DbContext, r => r
            .WithId(1)
            .WithEventId(1)
            .WithBuyerEmail("juan@test.com")
            .WithQuantity(2)
            .Confirmed()
            .WithReservationCode("EV-TEST-001"));

        var request = new GetByReservationCodeRequest("juan@test.com", "EV-TEST-001");

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.BuyerEmail.Should().Be("juan@test.com");
        result.Value.Quantity.Should().Be(2);
        result.Value.Status.Should().Be(ReservationStatus.Confirmada);
        result.Value.ReservationCode.Should().Be("EV-TEST-001");
        var eventDto = result.Value.Event;
        eventDto.Should().NotBeNull();
        eventDto!.Title.Should().Be("Rock Concert");
        eventDto.VenueId.Should().Be(venue.Id);
    }

    [Fact]
    public async Task GetByReservationCode_WhenReservationNotFound_ThenReturnsFailureNotFound()
    {
        // Arrange
        var state = new TestState();
        var request = new GetByReservationCodeRequest("unknown@test.com", "NONEXISTENT");

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(
            string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Reservation));
    }
}
