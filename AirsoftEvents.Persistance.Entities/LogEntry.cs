namespace AirsoftEvents.Persistance.Entities;

public record LogEntry(
    string id,
    DateTime timestampUtc,
    string? userId,
    string method,
    string path,
    int statusCode,
    int durationMs,
    string? traceId
);
