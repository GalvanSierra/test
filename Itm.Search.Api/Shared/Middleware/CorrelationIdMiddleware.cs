// En producción, este middleware viviría en un paquete NuGet interno compartido
// por todos los microservicios (Itm.Shared.Infrastructure).
// Se duplica aquí por simplicidad del monorepo educativo.

using System.Diagnostics;

namespace Itm.Search.Api.Shared.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            context.Response.Headers["X-Correlation-ID"] = correlationId;
            context.TraceIdentifier = correlationId;
            await _next(context);
        }
    }
}
