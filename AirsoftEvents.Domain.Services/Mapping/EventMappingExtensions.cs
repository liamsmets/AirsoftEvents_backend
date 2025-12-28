using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Models;
using AirsoftEvents.Domain.Models.Enums;
using System.Net.NetworkInformation;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using System.ComponentModel;
using System.Net;
using AirsoftEvents.Persistance.Entities;

namespace AirsoftEvents.Domain.Services.Mapping;

public static class EventMappingExtensions
{
    public static EventModel AsModel(this EventRequestContract request)
    {
        return new EventModel
        {
            Name = request.Name,
            Description = request.Description,
            Date = request.Date,
            Price = request.Price,
            MaxPlayers = request.MaxPlayers,
            FieldId = request.FieldId 
        };
    }

    public static Event AsEntity(this EventModel eventModel)
    {
        return new Event
        {
            Id = eventModel.Id,
            Name = eventModel.Name,
            Description = eventModel.Description,
            Date = eventModel.Date,
            Price = eventModel.Price,
            Status = eventModel.Status,
            FieldId = eventModel.FieldId,
            MaxPlayers = eventModel.MaxPlayers,
            UserId = eventModel.UserId
        };
    }

    public static EventModel AsModel(this Event eventEntity)
    {
        return new EventModel
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Date = eventEntity.Date,
            Price = eventEntity.Price,
            Status = eventEntity.Status,
            FieldId = eventEntity.FieldId ,
            MaxPlayers = eventEntity.MaxPlayers,
            UserId = eventEntity.UserId
        };
    }

    public static EventResponseContract AsContract(this EventModel model)
    {
        return new EventResponseContract
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Date = model.Date,
            Price = model.Price,
            Status = model.Status,
            FieldId = model.FieldId,
            MaxPlayers = model.MaxPlayers,
            UserId = model.UserId
        };
    }
}