using AirsoftEvents.Persistance.Entities;

namespace AirsoftEvents.Persistance.Interface;

public interface ILogRepo
{
    Task<LogEntry> CreateAsync(LogEntry entry);
    Task<LogEntry?> GetAsync(string id);
}
