using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.User.Login;
using EventsManager.Application.Tests.Common;
using EventsManager.Core.Constants;
using EventsManager.Core.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Features.User.Login;

public class LoginHandlerTests
{
    private sealed class TestState
    {
        public Mock<IUserRepository> UserRepositoryMock { get; } = new();
        public Mock<IPasswordHasher> PasswordHasherMock { get; } = new();
        public Mock<IJwtService> JwtServiceMock { get; } = new();
        public Mock<IValidator<LoginRequest>> ValidatorMock { get; } = new();
        public LoginHandler Handler { get; }
        public LoginRequestBuilder RequestBuilder { get; } = new();

        public TestState()
        {
            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            Handler = new LoginHandler(
                ValidatorMock.Object,
                UserRepositoryMock.Object,
                PasswordHasherMock.Object,
                JwtServiceMock.Object);
        }
    }

    [Fact]
    public async Task Login_WhenValidCredentials_ThenReturnsSuccessWithJwtToken()
    {
        // Arrange
        var state = new TestState();
        var request = state.RequestBuilder.Build();
        var userEntity = new UserEntityBuilder().Build();

        state.UserRepositoryMock
            .Setup(r => r.GetByUsernameAsync(request.Username))
            .ReturnsAsync(userEntity);

        state.PasswordHasherMock
            .Setup(p => p.Verify(request.Password, userEntity.PasswordHash))
            .Returns(true);

        state.JwtServiceMock
            .Setup(j => j.Generate(It.IsAny<JwtGenerateRequest>()))
            .Returns("jwt-token-value");

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("jwt-token-value");

        state.UserRepositoryMock.Verify(x => x.GetByUsernameAsync(request.Username), Times.Once);
        state.PasswordHasherMock.Verify(x => x.Verify(request.Password, userEntity.PasswordHash), Times.Once);
        state.JwtServiceMock.Verify(x => x.Generate(It.IsAny<JwtGenerateRequest>()), Times.Once);
    }

    [Fact]
    public async Task Login_WhenUserNotFound_ThenReturnsFailureCredentials()
    {
        // Arrange
        var state = new TestState();
        var request = state.RequestBuilder.Build();

        state.UserRepositoryMock
            .Setup(r => r.GetByUsernameAsync(request.Username))
            .ReturnsAsync((UserEntity?)null);

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.User.Error_Credentials);

        state.UserRepositoryMock.Verify(x => x.GetByUsernameAsync(request.Username), Times.Once);
        state.PasswordHasherMock.Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        state.JwtServiceMock.Verify(x => x.Generate(It.IsAny<JwtGenerateRequest>()), Times.Never);
    }

    [Fact]
    public async Task Login_WhenWrongPassword_ThenReturnsFailureCredentials()
    {
        // Arrange
        var state = new TestState();
        var request = state.RequestBuilder.Build();
        var userEntity = new UserEntityBuilder().Build();

        state.UserRepositoryMock
            .Setup(r => r.GetByUsernameAsync(request.Username))
            .ReturnsAsync(userEntity);

        state.PasswordHasherMock
            .Setup(p => p.Verify(request.Password, userEntity.PasswordHash))
            .Returns(false);

        // Act
        var result = await state.Handler.Execute(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SystemMessages.User.Error_Credentials);

        state.UserRepositoryMock.Verify(x => x.GetByUsernameAsync(request.Username), Times.Once);
        state.PasswordHasherMock.Verify(x => x.Verify(request.Password, userEntity.PasswordHash), Times.Once);
        state.JwtServiceMock.Verify(x => x.Generate(It.IsAny<JwtGenerateRequest>()), Times.Never);
    }
}
