using System.Net;
using System.Text.Json;
using OneMillionCopy.Leads.Application.Common.Exceptions;

namespace OneMillionCopy.Leads.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException exception)
        {
            context.Response.StatusCode = exception.StatusCode;
            context.Response.ContentType = "application/json";

            var payload = JsonSerializer.Serialize(new
            {
                error = exception.Message
            });

            await context.Response.WriteAsync(payload);
        }
        catch (Exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = JsonSerializer.Serialize(new
            {
                error = "Ocurrio un error interno en el servidor."
            });

            await context.Response.WriteAsync(payload);
        }
    }
}
