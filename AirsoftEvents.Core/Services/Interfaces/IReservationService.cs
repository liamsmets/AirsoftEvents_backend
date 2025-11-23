using AirsoftEvents.Core.Contracts;

namespace AirsoftEvents.Core.Services.Interfaces;

public interface IReservationService
{
    Task<ReservationResponseContract> CreateReservationAsync(ReservationRequestContract request);
    Task<ReservationResponseContract?> GetReservationByIdAsync(Guid id);
}