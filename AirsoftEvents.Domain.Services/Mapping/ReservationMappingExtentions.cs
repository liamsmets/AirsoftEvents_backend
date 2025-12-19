using System.Net;
using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Models;
using AirsoftEvents.Persistance.Entities;

namespace AirsoftEvents.Domain.Services.Mapping;

public static class ReservationMappingExtensions
{
    public static ReservationModel AsModel(this ReservationRequestContract request)
    {
        return new ReservationModel
        {
            EventId = request.EventId,
            UserId = request.UserId
        };
    }

    public static Reservation AsEntity(this ReservationModel reservationModel)
    {
        return new Reservation
        {
            Id = reservationModel.Id,
            EventId = reservationModel.EventId,
            UserId = reservationModel.UserId,
            ReservedAt = reservationModel.ReservedAt
        };
    }

    public static ReservationResponseContract AsContract(this ReservationModel model)
    {
        return new ReservationResponseContract
        {
            Id = model.Id,
            EventId = model.EventId,
            UserId = model.UserId,
            ReservedAt = model.ReservedAt
        };
    }

    public static ReservationModel AsModel(this Reservation reservationEntity)
    {
        return new ReservationModel
        {
            EventId = reservationEntity.EventId,
            UserId = reservationEntity.UserId
        };
    }
}