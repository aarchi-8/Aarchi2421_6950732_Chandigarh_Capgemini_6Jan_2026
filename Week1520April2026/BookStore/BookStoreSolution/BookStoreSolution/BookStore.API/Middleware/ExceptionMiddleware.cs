using System.Net;
using System.Text.Json;
namespace BookStore.API.Middleware;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next; private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) { _next = next; _logger = logger; }
    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (KeyNotFoundException ex) { _logger.LogWarning(ex, "Not found."); await Write(context, HttpStatusCode.NotFound, ex.Message); }
        catch (UnauthorizedAccessException) { await Write(context, HttpStatusCode.Unauthorized, "Unauthorized."); }
        catch (InvalidOperationException ex) { _logger.LogWarning(ex, "Bad request."); await Write(context, HttpStatusCode.BadRequest, ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Unhandled."); await Write(context, HttpStatusCode.InternalServerError, "Something went wrong."); }
    }
    private static async Task Write(HttpContext ctx, HttpStatusCode code, string msg) { ctx.Response.ContentType = "application/json"; ctx.Response.StatusCode = (int)code; await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { status = (int)code, message = msg })); }
}