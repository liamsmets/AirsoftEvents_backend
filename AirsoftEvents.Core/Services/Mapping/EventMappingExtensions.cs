using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Domain.Models;
using AirsoftEvents.Core.Domain.Enums;
using System.Net.NetworkInformation;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using System.ComponentModel;
using System.Net;

namespace AirsoftEvents.Core.Services.Mapping;

public static class EventMappingExtensions
{
    public static Event AsModel(this EventRequestContract request)
    {
        return new Event
        {
            Name = request.Name,
            Description = request.Description,
            Date = request.Date,
            Price = request.Price,
            MaxPlayers = request.MaxPlayers,
            FieldId = request.FieldId 
        };
    }

    public static EventResponseContract AsContract(this Event model)
    {
        return new EventResponseContract
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Date = model.Date,
            Price = model.Price,
            Status = model.Status.ToString(),
            FieldId = model.FieldId
        };
    }
}