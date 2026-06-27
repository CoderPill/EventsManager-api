using EventsManager.Application.Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EventsManager.Api.Extensions
{
    public static class ResultActionExtensions
    {
        extension<T>(Task<Result<T>> result)
        {
            public async Task<IActionResult> ToActionResult(HttpStatusCode successCode = HttpStatusCode.OK, HttpStatusCode failureCode = HttpStatusCode.BadRequest)
            {
                var r = await result;
                if (r.IsSuccess)
                {
                    return successCode switch
                    {
                        HttpStatusCode.OK => new OkObjectResult((Result<T>)r),
                        HttpStatusCode.Created => new CreatedResult(string.Empty, (Result<T>)r),
                        HttpStatusCode.NoContent => new NoContentResult(),
                        _ => new ObjectResult(r) { StatusCode = (int)successCode }
                    };
                }

                return failureCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestObjectResult((Result<T>)r),
                    HttpStatusCode.NotFound => new NotFoundObjectResult((Result<T>)r),
                    _ => new ObjectResult((Result<T>)r) { StatusCode = (int)failureCode }
                };
            }
        }
    }
}
