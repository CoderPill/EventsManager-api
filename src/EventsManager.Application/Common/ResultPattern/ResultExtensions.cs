using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Common.ResultPattern
{
    public static class ResultExtensions
    {

        extension<T>(Result<T> result)
        {
            public Task<Result<T>> ToTask() => Task.FromResult(result);
            public Result<U> Bind<U>(Func<T, Result<U>> func)
            {
                return result.IsSuccess
                    ? func(result.Value)
                    : Result.Failure<U>(result.Errors);
            }
            public async Task<Result<U>> BindAsync<U>(Func<T, Task<Result<U>>> func)
            {
                return result.IsSuccess
                    ? await func(result.Value)
                    : Result.Failure<U>(result.Errors);
            }
            public Result<U> Map<U>(Func<T, U> mapper)
            {
                return result.Bind(x => mapper(x).Success());
            }
            public async Task<Result<U>> MapAsync<U>(Func<T, Task<U>> mapper)
            {
                return await result.BindAsync(async x => (await mapper(x)).Success());
            }

        }

        extension<T>(Task<Result<T>> result)
        {
            public async Task<Result<U>> BindAsync<U>(Func<T, Task<Result<U>>> func)
            {
                var r = await result;
                return await r.BindAsync(func);
            }
            public async Task<Result<U>> Bind<U>(Func<T, Result<U>> func)
            {
                var r = await result;
                return r.Bind(func);
            }
            public async Task<Result<U>> MapAsync<U>(Func<T, Task<U>> mapper)
            {
                var r = await result;
                return await r.MapAsync(mapper);
            }
            public async Task<Result<U>> Map<U>(Func<T, U> mapper)
            {
                var r = await result;
                return r.Map(mapper);
            }
        }
    }
}
