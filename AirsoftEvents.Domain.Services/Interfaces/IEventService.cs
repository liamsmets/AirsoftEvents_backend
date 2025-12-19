using AirsoftEvents.Api.Contracts;


namespace AirsoftEvents.Domain.Services.Interfaces;

public interface IEventService
{
    Task<EventResponseContract> CreateEventAsync(EventRequestContract newEvent);
    Task<List<EventResponseContract>> GetUpcomingEventsAsync();
    Task<List<EventResponseContract>> GetAllEventsAsync();
    Task<EventResponseContract?> GetEventByIdAsync(Guid id);
    Task ApproveEventAsync(Guid id);
}