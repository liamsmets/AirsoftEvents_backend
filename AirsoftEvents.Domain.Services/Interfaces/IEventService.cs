using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Models.Enums;


namespace AirsoftEvents.Domain.Services.Interfaces;

public interface IEventService
{
    Task<EventResponseContract> CreateEventAsync(EventRequestContract newEvent, Guid userId);
    Task<List<EventResponseContract>> GetUpcomingEventsAsync(EventStatus eventStatus);
    Task<List<EventResponseContract>> GetAllEventsAsync();
    Task<List<EventResponseContract>> GetMyEventsAsync(Guid userId);
    Task<EventResponseContract?> GetEventByIdAsync(Guid id);
    Task ApproveEventAsync(Guid id);
}