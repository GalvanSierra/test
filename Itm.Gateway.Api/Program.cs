using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Configuración de YARP
// =========================
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// =========================
// Configuración de Rate Limiting
// =========================
builder.Services.AddRateLimiter(options =>
{
    // Código HTTP cuando se excede el límite
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Política de ventana fija
    options.AddPolicy("fixed-policy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,                  // Máximo 5 peticiones
                Window = TimeSpan.FromSeconds(10), // Cada 10 segundos
                QueueLimit = 0                    // No encolar solicitudes
            }));
});

var app = builder.Build();

// =========================
// Activar middleware
// =========================
app.UseRateLimiter();

// =========================
// Configurar Reverse Proxy
// =========================
app.MapReverseProxy()
    .RequireRateLimiting("fixed-policy");

app.Run();

//¿Qué hace esta configuración?
// Permite máximo 5 solicitudes por IP.
// El contador se reinicia cada 10 segundos.
// Si un cliente supera el límite:
//  recibe un HTTP 429 — Too Many Requests.
// No se almacenan solicitudes en cola (QueueLimit = 0).