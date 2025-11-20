using AirsoftEvents.Core.Domain.Models;

namespace AirsoftEvents.Core.Persistence.Interfaces;

public interface IEventRepo
{
    Task<List<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(Guid id);
    Task<List<Event>> GetApprovedEventsAsync();
    Task<Event> AddAsync(Event eventToAdd);
    Task UpdateAsync(Event eventToUpdate);
    Task DeleteAsync(Guid id);
}