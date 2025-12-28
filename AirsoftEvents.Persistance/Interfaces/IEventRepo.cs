using AirsoftEvents.Domain.Models;
using AirsoftEvents.Persistance.Entities;
namespace AirsoftEvents.Persistance.Interface;
public interface IEventRepo
{
    Task<List<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(Guid id);
    Task<List<Event>> GetApprovedEventsAsync();
    Task<List<Event>> GetByUserIdAsync(Guid id);
    Task<Event> AddAsync(Event eventToAdd);
    Task UpdateAsync(Event eventToUpdate);
    Task DeleteAsync(Guid id);
}