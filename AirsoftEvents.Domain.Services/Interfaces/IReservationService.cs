using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Models.Enums;

namespace AirsoftEvents.Domain.Services.Interfaces;

public interface IReservationService
{
    Task<ReservationResponseContract> CreateReservationAsync(ReservationRequestContract request, Guid userId);
    Task<ReservationResponseContract?> GetReservationByIdAsync(Guid id);
    Task UpdatePaymentStatusAsync(Guid id, string molliePaymentId, ReservationpaymentStatus status);
    Task<ReservationResponseContract?>GetByMolliePaymentIdAsync(string id);
}