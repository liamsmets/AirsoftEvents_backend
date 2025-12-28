using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Models;
using AirsoftEvents.Persistance.Entities;

namespace AirsoftEvents.Domain.Services.Mapping;

public static class FieldMappingExtentions
{
    public static AirsoftFieldModel AsModel(this FieldRequestContract request)
    {
        return new AirsoftFieldModel
        {
            Name = request.Name,
            Address = request.Address,
            Description = request.Description,
            Capacity = request.Capacity,
            OwnerId = request.OwnerId
        };
    }

    public static Field AsEntity(this AirsoftFieldModel fieldModel)
    {
        return new Field
        {
            Id = fieldModel.Id,
            Name = fieldModel.Name,
            Address = fieldModel.Address,
            Description = fieldModel.Description,
            Capacity = fieldModel.Capacity,
            OwnerId = fieldModel.OwnerId,
            Status = fieldModel.Status
        };
    }

    public static AirsoftFieldModel AsModel(this Field fieldEntity)
    {
        return new AirsoftFieldModel
        {
            Id = fieldEntity.Id,
            Name = fieldEntity.Name,
            Address = fieldEntity.Address,
            Description = fieldEntity.Description,
            Capacity = fieldEntity.Capacity,
            OwnerId = fieldEntity.OwnerId,
            Status = fieldEntity.Status
        };
    }
    public static FieldResponseContract AsContract(this AirsoftFieldModel model)
    {
        return new FieldResponseContract
        {
            Id = model.Id,
            Name = model.Name,
            Address = model.Address,
            Description = model.Description,
            Capacity = model.Capacity,
            Status = model.Status,
            OwnerId = model.OwnerId
        };
    }
}