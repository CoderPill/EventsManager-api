using EventsManager.Application.Common.DTOs;
using System.Collections.Immutable;

namespace EventsManager.Application.Common.ResultPattern
{
    public struct Result<T>
    {
        public bool IsSuccess => Errors.IsDefaultOrEmpty && _isInitialized;
        public T Value { get; }
        public ImmutableArray<string> Errors { get; }
        private readonly bool _isInitialized;

        internal Result(T value)
        {
            _isInitialized = true;
            Value = value;
            Errors = ImmutableArray<string>.Empty;
        }

        internal Result(ImmutableArray<string> errors)
        {
            _isInitialized = true;
            Value = default(T)!;
            Errors = errors;
        }

        public static implicit operator Result<T>(T value) => new(value);
    }

    public static class Result
    {
        public static Result<T> Success<T>(this T value) => new(value);
        public static Result<T> Failure<T>(ImmutableArray<string> errors) => new(errors);
        public static Result<T> Failure<T>(string error) => new(ImmutableArray.Create<string>(error));
        public static Result<T> Failure<T>(params string[] errors) => new(ImmutableArray.Create(errors));

        public static readonly Unit Unit = Unit.Value;
        public static Result<Unit> Success() => new(Unit);
        public static Result<Unit> Failure(ImmutableArray<string> errors) => new(errors);
        public static Result<Unit> Failure(string error) => new(ImmutableArray.Create<string>(error));
        public static Result<Unit> Failure(params string[] errors) => new(ImmutableArray.Create(errors));
    }
}
