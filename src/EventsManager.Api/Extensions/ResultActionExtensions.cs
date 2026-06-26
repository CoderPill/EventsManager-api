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
                        HttpStatusCode.OK => new OkObjectResult(r.Value),
                        HttpStatusCode.Created => new CreatedResult(string.Empty, r.Value),
                        HttpStatusCode.NoContent => new NoContentResult(),
                        _ => new ObjectResult(r.Value) { StatusCode = (int)successCode }
                    };
                }

                return failureCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestObjectResult(r.Errors),
                    HttpStatusCode.NotFound => new NotFoundObjectResult(r.Errors),
                    _ => new ObjectResult(r.Errors) { StatusCode = (int)failureCode }
                };
            }
        }
    }
}
