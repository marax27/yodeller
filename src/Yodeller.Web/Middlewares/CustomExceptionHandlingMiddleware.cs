using System.Net;

namespace Yodeller.Web.Middlewares;

public class CustomExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ArgumentException exc)
        {
            await ProcessException(httpContext, exc.Message, HttpStatusCode.UnprocessableEntity);
        }
        catch (NotImplementedException exc)
        {
            await ProcessException(httpContext, exc.Message, HttpStatusCode.NotImplemented);
        }
    }

    private static async Task ProcessException(HttpContext httpContext, string message, HttpStatusCode statusCode)
    {
        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "text/plain";
        await httpContext.Response.WriteAsync(message);
    }
}
