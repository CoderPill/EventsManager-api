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
                var response = await result;

                if (response.IsSuccess)
                {
                    return successCode switch
                    {
                        HttpStatusCode.Created => new CreatedResult(string.Empty, response),
                        _ => new OkObjectResult(response)
                    };
                }

                return failureCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestObjectResult(Result.Failure(response.Errors)),
                    HttpStatusCode.NotFound => new NotFoundObjectResult(Result.Failure(response.Errors)),
                    HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(Result.Failure(response.Errors)),
                    HttpStatusCode.Conflict => new ConflictObjectResult(Result.Failure(response.Errors)),
                    _ => new ObjectResult(Result.Failure(response.Errors)) { StatusCode = (int)failureCode }
                };
            }
        }
    }
}
