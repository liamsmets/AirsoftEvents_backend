using AirsoftEvents.Api.Contracts;

namespace AirsoftEvents.Domain.Services.Interfaces;

public interface IReservationService
{
    Task<ReservationResponseContract> CreateReservationAsync(ReservationRequestContract request);
    Task<ReservationResponseContract?> GetReservationByIdAsync(Guid id);
}