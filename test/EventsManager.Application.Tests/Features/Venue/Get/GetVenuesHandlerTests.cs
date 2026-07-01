using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Venue;
using EventsManager.Application.Features.Venue.Get;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace EventsManager.Application.Tests.Features.Venue.Get;

public class GetVenuesHandlerTests
{
    private sealed class TestState
    {
        public Mock<IVenueRepository> VenueRepositoryMock { get; } = new();
        public Mock<IValidator<Unit>> ValidatorMock { get; } = new();
        public GetVenuesHandler Handler { get; }

        public TestState()
        {
            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<Unit>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new GetVenuesHandler(ValidatorMock.Object, VenueRepositoryMock.Object);
        }
    }

    [Fact]
    public async Task GetVenues_WhenVenuesExist_ThenReturnsListOfVenueDtos()
    {
        // Arrange
        var state = new TestState();
        var venues = new List<VenueEntity>
        {
            new VenueEntityBuilder().WithId(1).WithName("Venue 1").Build(),
            new VenueEntityBuilder().WithId(2).WithName("Venue 2").WithCapacity(50).WithCity("Medellín").Build()
        };

        state.VenueRepositoryMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<VenueEntity, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<string[]>()))
            .ReturnsAsync(venues);

        // Act
        var result = await state.Handler.Execute(Unit.Value);

        // Assert — output assertions prueban que el handler funciona, no verificamos llamadas internas (Principio 7)
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Id.Should().Be(1);
        result.Value[1].Id.Should().Be(2);
    }
}
