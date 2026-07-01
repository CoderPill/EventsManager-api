using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.User.Logout;
using EventsManager.Application.Tests.Common;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.User.Logout;

public class LogoutHandlerTests
{
    private sealed class TestState
    {
        public Mock<IJwtService> JwtServiceMock { get; } = new();
        public Mock<IValidator<LogoutRequest>> ValidatorMock { get; } = new();
        public LogoutHandler Handler { get; }
        public LogoutRequestBuilder RequestBuilder { get; } = new();

        public TestState()
        {
            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<LogoutRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new LogoutHandler(ValidatorMock.Object, JwtServiceMock.Object);
        }
    }

    [Fact]
    public async Task Logout_WhenValidJti_ThenRevokesTokenSuccessfully()
    {
        // Arrange
        var state = new TestState();
        var request = state.RequestBuilder.Build();

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        state.JwtServiceMock.Verify(x => x.Revoke(
            It.Is<JwtRevokeRequest>(r => r.Jti == request.Jti && r.ExpiresDate == request.ExpiresDate)
        ), Times.Once);
    }
}
