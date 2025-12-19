using AirsoftEvents.Domain.Models.Enums;
using AirsoftEvents.Domain.Models;
using AirsoftEvents.Domain.Services.Exceptions;
using AirsoftEvents.Persistance.Interface;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Domain.Services.Mapping;
using AirsoftEvents.Api.Contracts;

namespace AirsoftEvents.Domain.Services;

public class EventService (IEventRepo eventRepo, IFieldRepo fieldRepo) : IEventService
{
    
    public async Task<EventResponseContract> CreateEventAsync(EventRequestContract eventRequest)
    {
        var newEvent = eventRequest.AsModel().AsEntity();

        var field = await fieldRepo.GetByIdAsync(newEvent.FieldId);
        
        if (field == null) 
            throw new ArgumentException("Terrein niet gevonden");

        if (field.Status != FieldStatus.Approved)
            throw new TerrainNotApprovedException("Terrein is niet goedgekeurd");

        if (newEvent.MaxPlayers > field.Capacity)
            throw new CapacityExceededException("Te veel spelers voor dit terrein");

        newEvent.Id = Guid.NewGuid();
        newEvent.Status = EventStatus.Pending;

        var createdEvent = await eventRepo.AddAsync(newEvent);

 
        return createdEvent.AsModel().AsContract();
    }

    public async Task<List<EventResponseContract>> GetUpcomingEventsAsync()
    {
        var events = await eventRepo.GetApprovedEventsAsync();
    
        return events.Select(e => e.AsModel().AsContract()).ToList();
    }

    public async Task<List<EventResponseContract>> GetAllEventsAsync()
    {
        var events = await eventRepo.GetAllAsync();
        return events.Select(e => e.AsModel().AsContract()).ToList();
    }

    public async Task<EventResponseContract?> GetEventByIdAsync(Guid id)
    {
        var eventModel = await eventRepo.GetByIdAsync(id);

        if (eventModel == null)
        {
            return null;
        }

        return eventModel.AsModel().AsContract();
    }

    public async Task ApproveEventAsync(Guid id)
    {
        var eventModel = await eventRepo.GetByIdAsync(id);

        if (eventModel == null)
        {
            throw new KeyNotFoundException("Event niet gevonden.");
        }

        eventModel.Status = EventStatus.Approved;

        await eventRepo.UpdateAsync(eventModel);
    }
}