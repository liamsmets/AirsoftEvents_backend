using AirsoftEvents.Core.Domain.Enums;
using AirsoftEvents.Core.Domain.Models;
using AirsoftEvents.Core.Exceptions;
using AirsoftEvents.Core.Persistence.Interfaces;
using AirsoftEvents.Core.Services.Interfaces;
using AirsoftEvents.Core.Services.Mapping;
using AirsoftEvents.Core.Contracts;

namespace AirsoftEvents.Core.Services.Implementations;

public class EventService : IEventService
{
    private readonly IEventRepo _eventRepo;
    private readonly IFieldRepo _fieldRepo;

    public EventService(IEventRepo eventRepo, IFieldRepo fieldRepo)
    {
        _eventRepo = eventRepo;
        _fieldRepo = fieldRepo;
    }

    public async Task<EventResponseContract> CreateEventAsync(EventRequestContract eventRequest)
    {
        var newEvent = eventRequest.AsModel();

        var field = await _fieldRepo.GetByIdAsync(newEvent.FieldId);
        
        if (field == null) 
            throw new ArgumentException("Terrein niet gevonden");

        if (field.Status != FieldStatus.Approved)
            throw new TerrainNotApprovedException("Terrein is niet goedgekeurd");

        if (newEvent.MaxPlayers > field.Capacity)
            throw new CapacityExceededException("Te veel spelers voor dit terrein");

        newEvent.Id = Guid.NewGuid();
        newEvent.Status = EventStatus.Pending;

        var createdEvent = await _eventRepo.AddAsync(newEvent);
 
        return createdEvent.AsContract();
    }

    public async Task<List<EventResponseContract>> GetUpcomingEventsAsync()
    {
        var events = await _eventRepo.GetApprovedEventsAsync();
    
        return events.Select(e => e.AsContract()).ToList();
    }

    public async Task<List<EventResponseContract>> GetAllEventsAsync()
    {
        var events = await _eventRepo.GetAllAsync();
        return events.Select(e => e.AsContract()).ToList();
    }

    public async Task<EventResponseContract?> GetEventByIdAsync(Guid id)
    {
        var eventModel = await _eventRepo.GetByIdAsync(id);

        if (eventModel == null)
        {
            return null;
        }

        return eventModel.AsContract();
    }

    public async Task ApproveEventAsync(Guid id)
    {
        var eventModel = await _eventRepo.GetByIdAsync(id);

        if (eventModel == null)
        {
            throw new KeyNotFoundException("Event niet gevonden.");
        }

        eventModel.Status = EventStatus.Approved;

        await _eventRepo.UpdateAsync(eventModel);
    }
}