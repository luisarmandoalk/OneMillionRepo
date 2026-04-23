using System.Net;
using System.Text.Json;
using OneMillionCopy.Leads.Application.Common.Exceptions;

namespace OneMillionCopy.Leads.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
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
        catch (Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            object payloadObject = _environment.IsDevelopment()
                ? new
                {
                    error = "Ocurrio un error interno en el servidor.",
                    detail = exception.Message
                }
                : new
                {
                    error = "Ocurrio un error interno en el servidor."
                };

            var payload = JsonSerializer.Serialize(payloadObject);

            await context.Response.WriteAsync(payload);
        }
    }
}
