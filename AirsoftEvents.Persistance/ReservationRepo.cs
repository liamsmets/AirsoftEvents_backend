using AirsoftEvents.Persistance.Interface;
using AirsoftEvents.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using AirsoftEvents.Domain.Models.Enums;
using System.Net;
using System.Security.Cryptography;

namespace AirsoftEvents.Persistance;

public class ReservationRepo(AirsoftEventsAppDbContext dbContext): IReservationRepo
{
    public async Task<List<Reservation>> GetAllAsync()
    {
        return await dbContext.Reservations.ToListAsync();
    }
    public async Task<Reservation?> GetByIdAsync(Guid id)
    {
        return await dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == id);

    }
    public async Task<List<Reservation>> GetByEventIdAsync(Guid id)
    {
        return await dbContext.Reservations.Where(r => r.EventId == id).ToListAsync();
    }
    public async Task<List<Reservation>> GetByUserIdAsync(Guid id)
    {
        return await dbContext.Reservations.Where(r => r.UserId == id).ToListAsync();
    }
    public async Task<Reservation> AddAsync(Reservation reservationToAdd)
    {
        await dbContext.Reservations.AddAsync(reservationToAdd);
        await dbContext.SaveChangesAsync();
        return reservationToAdd;
    }
    public async Task UpdateAsync(Reservation reservationToUpdate)
    {
        dbContext.Reservations.Update(reservationToUpdate);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var reservationToDelete = await dbContext.Reservations.FindAsync(id);
        if(reservationToDelete != null)
        {
            dbContext.Reservations.Remove(reservationToDelete);
            await dbContext.SaveChangesAsync();
        }
    }
}