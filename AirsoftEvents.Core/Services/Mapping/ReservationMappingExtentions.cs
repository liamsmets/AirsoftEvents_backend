using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Domain.Models;

namespace AirsoftEvents.Core.Services.Mapping;

public static class ReservationMappingExtensions
{
    public static Reservation AsModel(this ReservationRequestContract request)
    {
        return new Reservation
        {
            EventId = request.EventId,
            UserId = request.UserId
        };
    }

    public static ReservationResponseContract AsContract(this Reservation model)
    {
        return new ReservationResponseContract
        {
            Id = model.Id,
            EventId = model.EventId,
            UserId = model.UserId,
            ReservedAt = model.ReservedAt
        };
    }
}