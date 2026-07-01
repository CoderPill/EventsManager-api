using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace EventsManager.Application.Tests.Common;

public class BaseUseCaseTests
{
    private sealed class TestDoubleUseCase : BaseUseCase<Unit, Unit>
    {
        public bool OnExecuteCalled { get; private set; }

        public TestDoubleUseCase(IValidator<Unit> validator) : base(validator) { }

        protected override Task<Result<Unit>> OnExecute(Unit request)
        {
            OnExecuteCalled = true;
            return Task.FromResult(Result.Success());
        }
    }

    [Fact]
    public async Task BaseUseCase_WhenValidatorRejects_ThenReturnsFailure()
    {
        // Arrange
        const string expectedErrorMessage = "La propiedad Unit es requerida";

        var validatorMock = new Mock<IValidator<Unit>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<Unit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Unit", expectedErrorMessage)
            }));

        var testDouble = new TestDoubleUseCase(validatorMock.Object);

        // Act
        var result = await testDouble.Execute(Unit.Value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(expectedErrorMessage);
        testDouble.OnExecuteCalled.Should().BeFalse();
    }
}
