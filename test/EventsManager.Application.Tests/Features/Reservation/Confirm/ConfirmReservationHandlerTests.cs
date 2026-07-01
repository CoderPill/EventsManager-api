using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Reservation.Confirm;
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

namespace EventsManager.Application.Tests.Features.Reservation.Confirm;

public class ConfirmReservationHandlerTests : HandlerTestBase
{
    private sealed class TestState
    {
        private readonly string _databaseName;

        public DbContextEventsManager DbContext { get; }
        public IEventRepository EventRepository { get; }
        public IReservationRepository ReservationRepository { get; }
        public FakeTimeProvider TimeProvider { get; } = new();
        public Mock<IValidator<ConfirmReservationRequest>> ValidatorMock { get; } = new();
        public Mock<IAlphaNumericCodeGenerator> CodeGeneratorMock { get; } = new();
        public Mock<IEmailService> EmailServiceMock { get; } = new();
        public ConfirmReservationHandler Handler { get; }
        public ConfirmReservationRequestBuilder RequestBuilder { get; } = new();

        /// <summary>
        /// Creates a fresh DbContext that reads from the same InMemory store independently,
        /// proving that data was actually persisted via SaveChangesAsync.
        /// </summary>
        public DbContextEventsManager CreateReaderContext() =>
            CreateInMemoryDbContext(_databaseName);

        public TestState()
        {
            _databaseName = Guid.NewGuid().ToString();
            DbContext = CreateInMemoryDbContext(_databaseName);
            EventRepository = new EventRepository(DbContext, TimeProvider);
            ReservationRepository = new ReservationRepository(DbContext, TimeProvider);

            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ConfirmReservationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            CodeGeneratorMock
                .Setup(cg => cg.Generate(6))
                .Returns("ABCDEF");

            EmailServiceMock
                .Setup(e => e.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            Handler = new ConfirmReservationHandler(
                ValidatorMock.Object,
                ReservationRepository,
                EventRepository,
                CodeGeneratorMock.Object,
                EmailServiceMock.Object,
                TimeProvider);
        }
    }

    [Fact]
    public async Task ConfirmReservation_WhenPendingReservation_ThenConfirmsSendsEmailGeneratesCode()
    {
        // Arrange
        var state = new TestState();
        await SeedEventAsync(state.DbContext, e => e.WithId(1));

        await SeedReservationAsync(state.DbContext, r => r
            .WithId(1)
            .WithEventId(1)
            .WithStatus(ReservationStatus.PendientePago)
            .WithQuantity(2));

        var request = state.RequestBuilder
            .WithReservationId(1)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("EV-ABCDEF");

        state.CodeGeneratorMock.Verify(cg => cg.Generate(6), Times.AtLeastOnce);

        state.EmailServiceMock.Verify(
            e => e.SendAsync(
                It.Is<EmailMessage>(m =>
                    m.To == "juan@test.com" &&
                    m.Subject.Contains("Test Event") &&
                    m.Body.Contains("EV-ABCDEF")),
                It.IsAny<CancellationToken>()),
            Times.Once);

        // Use a separate reader context to prove SaveChangesAsync persisted the data
        // (same DbContext would return the tracked instance regardless of persistence)
        using var readerContext = state.CreateReaderContext();
        var savedReservation = await readerContext.Reservations.FindAsync(1);
        savedReservation.Should().NotBeNull();
        savedReservation!.Status.Should().Be(ReservationStatus.Confirmada);
        savedReservation.ReservationCode.Should().Be("EV-ABCDEF");
    }

    [Fact]
    public async Task ConfirmReservation_WhenNotFound_ThenReturnsFailureNotFound()
    {
        // Arrange
        var state = new TestState();
        var request = state.RequestBuilder
            .WithReservationId(999)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(
            string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Reservation));
    }

    [Fact]
    public async Task ConfirmReservation_WhenAlreadyCancelled_ThenReturnsFailureAlreadyCancelled()
    {
        // Arrange
        var state = new TestState();
        await SeedEventAsync(state.DbContext, e => e.WithId(1));

        await SeedReservationAsync(state.DbContext, r => r
            .WithId(1)
            .WithEventId(1)
            .Cancelled());

        var request = state.RequestBuilder
            .WithReservationId(1)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_ReservationAlreadyCancelled);

        state.EmailServiceMock.Verify(
            e => e.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmReservation_WhenAlreadyConfirmed_ThenReturnsFailureAlreadyConfirmed()
    {
        // Arrange
        var state = new TestState();
        await SeedEventAsync(state.DbContext, e => e.WithId(1));

        await SeedReservationAsync(state.DbContext, r => r
            .WithId(1)
            .WithEventId(1)
            .Confirmed());

        var request = state.RequestBuilder
            .WithReservationId(1)
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_ReservationAlreadyConfirmed);

        state.EmailServiceMock.Verify(
            e => e.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmReservation_WhenCodeGeneratorFails5Times_ThenReturnsFailureCodeGeneration()
    {
        // Arrange
        var state = new TestState();
        await SeedEventAsync(state.DbContext, e => e.WithId(1));

        // Seed a reservation with the same code the generator will produce,
        // causing ExistsByCodeAsync to return true on every attempt
        await SeedReservationAsync(state.DbContext, r => r
            .WithId(1)
            .WithEventId(1)
            .WithReservationCode("EV-ABCDEF"));

        var request = state.RequestBuilder
            .WithReservationId(2) // different ID so ConfirmReservation won't find the seeded one
            .Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.Validations.Rule_CouldNotGenerateReservationCode);

        state.CodeGeneratorMock.Verify(cg => cg.Generate(6), Times.Exactly(5));

        state.EmailServiceMock.Verify(
            e => e.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmReservation_WhenValidatorRejects_ThenReturnsFailure()
    {
        // Arrange
        const string expectedError = "La propiedad ReservationId es requerida";

        var state = new TestState();
        state.ValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ConfirmReservationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("ReservationId", expectedError)
            }));

        var request = state.RequestBuilder.Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(expectedError);

        state.CodeGeneratorMock.Verify(cg => cg.Generate(It.IsAny<int>()), Times.Never);
        state.EmailServiceMock.Verify(e => e.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
