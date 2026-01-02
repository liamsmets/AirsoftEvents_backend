using System.Diagnostics;
using AirsoftEvents.Api.Extensions;
using AirsoftEvents.Persistance.Entities;          // jouw LogEntry.cs namespace
using AirsoftEvents.Persistance.Interface;         // jouw ILogRepo namespace

namespace AirsoftEvents.Api.Middleware;

public class CosmosLogMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogRepo logRepo)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();

            var path = context.Request.Path.ToString();

            if (!path.StartsWith("/swagger"))
            {
                string? userId = null;
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    userId = context.User.GetUserId().ToString();
                }

                var entry = new LogEntry(
                    id: Guid.NewGuid().ToString("N"),
                    timestampUtc: DateTime.UtcNow,
                    userId: userId,
                    method: context.Request.Method,
                    path: path,
                    statusCode: context.Response.StatusCode,
                    durationMs: (int)sw.ElapsedMilliseconds,
                    traceId: context.TraceIdentifier
                );

                await logRepo.CreateAsync(entry);
            }
        }
    }
}
