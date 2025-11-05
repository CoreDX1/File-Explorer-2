namespace Web.Middleware;

public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string RequestIdHeader = "X-Request-ID";

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId =
            context.Request.Headers[RequestIdHeader].FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Items["RequestId"] = requestId;
        context.Response.Headers[RequestIdHeader] = requestId;

        using (Serilog.Context.LogContext.PushProperty("RequestId", requestId))
        {
            await _next(context);
        }
    }
}
