using AirsoftEvents.Persistance.Interface;
using AirsoftEvents.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using AirsoftEvents.Domain.Models.Enums;
using System.Net;

namespace AirsoftEvents.Persistance;

public class EventRepo(AirsoftEventsAppDbContext dbContext): IEventRepo
{
    public async Task<List<Event>> GetAllAsync()
    {
        return await dbContext.Events.ToListAsync();
    }
    public async Task<Event?> GetByIdAsync(Guid id)
    {
        return await dbContext.Events.FirstOrDefaultAsync(e=> e.Id == id);
    }
    public async Task<List<Event>> GetApprovedEventsAsync()
    {
        return await dbContext.Events.Where(e => e.Status == EventStatus.Approved).ToListAsync();
    }
    public async Task<Event> AddAsync(Event eventToAdd)
    {
        await dbContext.Events.AddAsync(eventToAdd);
        await dbContext.SaveChangesAsync();
        return eventToAdd;
    }
    public async Task UpdateAsync(Event eventToUpdate)
    {
        dbContext.Events.Update(eventToUpdate);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var eventToDelete = await dbContext.Events.FindAsync(id);
        if(eventToDelete != null)
        {
            dbContext.Events.Remove(eventToDelete);
            await dbContext.SaveChangesAsync();
        }
    }
}