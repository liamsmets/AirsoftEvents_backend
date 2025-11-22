using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Domain.Models;

namespace AirsoftEvents.Core.Services.Mapping;

public static class FieldMappingExtentions
{
    public static AirsoftField AsModel(this FieldRequestContract request)
    {
        return new AirsoftField
        {
            Name = request.Name,
            Address = request.Address,
            Description = request.Description,
            Capacity = request.Capacity,
            OwnerId = request.OwnerId
        };
    }

    public static FieldResponseContract AsContract(this AirsoftField model)
    {
        return new FieldResponseContract
        {
            Id = model.Id,
            Name = model.Name,
            Address = model.Address,
            Capacity = model.Capacity,
            Status = model.Status.ToString()
        };
    }
}