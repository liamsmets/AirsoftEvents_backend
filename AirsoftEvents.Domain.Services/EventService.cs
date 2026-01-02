using AirsoftEvents.Domain.Models.Enums;
using AirsoftEvents.Domain.Models;
using AirsoftEvents.Domain.Services.Exceptions;
using AirsoftEvents.Persistance.Interface;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Domain.Services.Mapping;
using AirsoftEvents.Api.Contracts;

namespace AirsoftEvents.Domain.Services;

public class EventService (IEventRepo eventRepo, IFieldRepo fieldRepo, IUserRepo userRepo, IReservationRepo reservationRepo) : IEventService
{
    
    public async Task<EventResponseContract> CreateEventAsync(EventRequestContract eventRequest, Guid userId)
    {
        var user = await userRepo.GetByIdAsync(userId);
        
        

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

        newEvent.UserId = userId;

        var createdEvent = await eventRepo.AddAsync(newEvent);
        return createdEvent.AsModel().AsContract();
    }


    public async Task<List<EventResponseContract>> GetUpcomingEventsAsync(EventStatus eventStatus)
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
    public async Task<List<EventResponseContract>> GetMyEventsAsync(Guid userId)
    {
        var events = await eventRepo.GetByUserIdAsync(userId);
        return events.Select(e => e.AsModel().AsContract()).ToList();
    }

    public async Task<EventResponseContract> UpdateEventAsync(Guid id, EventUpdateContract update, Guid userId, bool isAdmin)
    {
        var ev = await eventRepo.GetByIdAsync(id);
        if (ev == null) throw new KeyNotFoundException();

        if (!isAdmin && ev.UserId != userId)
            throw new ForbiddenException("Je kan enkel je eigen event aanpassen.");

        ev.Name = update.Name;
        ev.Description = update.Description;
        ev.Date = update.Date;
        ev.Price = update.Price;
        ev.MaxPlayers = update.MaxPlayers;
        ev.FieldId = update.FieldId;

        await eventRepo.UpdateAsync(ev);
        return ev.AsModel().AsContract();
    }

    public async Task DeleteEventAsync(Guid id, Guid userId, bool isAdmin)
    {
        var ev = await eventRepo.GetByIdAsync(id);
        if (ev == null) throw new KeyNotFoundException();

        if (!isAdmin && ev.UserId != userId)
            throw new ForbiddenException("Je kan enkel je eigen event verwijderen.");

        await eventRepo.DeleteAsync(id);
    }

    public async Task<EventAvailabilityResponseContract> GetAvailabilityAsync(Guid eventId)
    {
        var ev = await eventRepo.GetByIdAsync(eventId);
        if (ev == null) throw new KeyNotFoundException("Event not found");

        var reserved = await reservationRepo.CountActiveByEventIdAsync(eventId);

        var free = ev.MaxPlayers - reserved;
        if (free < 0) free = 0;

        return new EventAvailabilityResponseContract
        {
            EventId = eventId,
            MaxPlayers = ev.MaxPlayers,
            Reserved = reserved,
            Free = free
        };
    }
    

    public async Task RejectEventAsync(Guid id)
    {
        var ev = await eventRepo.GetByIdAsync(id);
        if (ev is null) throw new KeyNotFoundException("Event not found");

        ev.Status = EventStatus.Rejected; // of hoe je enum heet
        await eventRepo.UpdateAsync(ev);
    }
}