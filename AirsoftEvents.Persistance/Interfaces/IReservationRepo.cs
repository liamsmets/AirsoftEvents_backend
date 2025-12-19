using AirsoftEvents.Domain.Models;
using AirsoftEvents.Persistance.Entities;
namespace AirsoftEvents.Persistance.Interface;

public interface IReservationRepo
{
    Task<List<Reservation>> GetAllAsync();
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<List<Reservation>> GetByEventIdAsync(Guid eventId);
    Task<List<Reservation>> GetByUserIdAsync(Guid userId);
    Task<Reservation> AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
    Task DeleteAsync(Guid id);
}