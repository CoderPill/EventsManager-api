using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Venue;
using EventsManager.Application.Features.Venue.Add;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.Venue.Add;

public class AddVenueHandlerTests
{
    private sealed class TestState
    {
        public Mock<IVenueRepository> VenueRepositoryMock { get; } = new();
        public Mock<IValidator<AddVenueRequest>> ValidatorMock { get; } = new();
        public AddVenueHandler Handler { get; }
        public AddVenueRequestBuilder RequestBuilder { get; } = new();

        public TestState()
        {
            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<AddVenueRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new AddVenueHandler(ValidatorMock.Object, VenueRepositoryMock.Object);
        }
    }

    [Fact]
    public async Task AddVenue_WhenValidRequest_ThenReturnsSuccessWithVenueDto()
    {
        // Arrange
        var state = new TestState();
        var request = state.RequestBuilder.Build();

        state.VenueRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<VenueEntity>()))
            .Callback<VenueEntity>(e => e.Id = 1)
            .Returns(Task.CompletedTask);

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Capacity.Should().Be(request.Capacity);
        result.Value.Id.Should().Be(1);

        state.VenueRepositoryMock.Verify(x => x.AddAsync(It.IsAny<VenueEntity>()), Times.Once);
        state.VenueRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
