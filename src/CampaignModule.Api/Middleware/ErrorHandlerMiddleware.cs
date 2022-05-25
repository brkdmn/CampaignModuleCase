using System.Net;
using System.Text.Json;
using CampaignModule.Core.Helper;
using CampaignModule.Domain.Response;

namespace CampaignModule.Api.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = error switch
            {
                AppException e =>
                    // custom application error
                    (int)e.StatusCode,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var result = JsonSerializer.Serialize(new BaseResponse<object>
            {
                Message = error.Message,
                StatusCode = response.StatusCode,
                IsSuccess = false,
                Result = null
            });
            await response.WriteAsync(result);
        }
    }
}