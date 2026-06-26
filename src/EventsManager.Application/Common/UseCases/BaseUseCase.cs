using EventsManager.Application.Common.ResultPattern;
using FluentValidation;
using System.Collections.Immutable;

namespace EventsManager.Application.Common.UseCases
{
    public abstract class BaseUseCase<TRequest, TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        protected BaseUseCase(IValidator<TRequest> validator) => _validator = validator;

        public async Task<Result<TResponse>> Execute(TRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Result.Failure<TResponse>(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToImmutableArray()
                );
            }

            return await OnExecute(request);
        }

        protected abstract Task<Result<TResponse>> OnExecute(TRequest request);
    }
}