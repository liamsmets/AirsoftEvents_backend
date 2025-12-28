using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Models.Enums;
using AirsoftEvents.Persistance.Interface;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Domain.Services.Mapping;
using AirsoftEvents.Persistance;
using AirsoftEvents.Domain.Models;
using AirsoftEvents.Persistance.Entities;
using AirsoftEvents.Domain.Services.Exceptions;

namespace AirsoftEvents.Domain.Services;

public class FieldService : IFieldService
{
    private readonly IFieldRepo _fieldRepo;

    public FieldService(IFieldRepo fieldRepo)
    {
        _fieldRepo = fieldRepo;
    }

    public async Task<FieldResponseContract> CreateFieldAsync(FieldRequestContract request, Guid ownerId)
    {
        var field = request.AsModel();
        field.Id = Guid.NewGuid();
        field.Status = FieldStatus.Pending;

        var entity = field.AsEntity();
        entity.OwnerId = ownerId; 

        var createdField = await _fieldRepo.AddAsync(entity);
        return createdField.AsModel().AsContract();
    }

    public async Task<List<FieldResponseContract>> GetAllFieldsAsync()
    {
        var fields = await _fieldRepo.GetAllAsync();
        return fields.Select(f => f.AsModel().AsContract()).ToList();
    }

    public async Task<FieldResponseContract?> GetFieldByIdAsync(Guid id)
    {
        var field = await _fieldRepo.GetByIdAsync(id);

        if (field == null)
        {
            return null;
        }

        return field.AsModel().AsContract();
    }
    public async Task<List<FieldResponseContract>> GetFieldByOwnerIdAsync(Guid id)
    {
        var fields = await _fieldRepo.GetByOwnerId(id);
        return fields.Select(f => f.AsModel().AsContract()).ToList();
    }
    public async Task ApproveFieldAsync(Guid id)
    {
        var fieldModel = await _fieldRepo.GetByIdAsync(id);

        if (fieldModel == null)
        {
            throw new KeyNotFoundException("Event niet gevonden.");
        }

        fieldModel.Status = FieldStatus.Approved;

        await _fieldRepo.UpdateAsync(fieldModel);
    }

    public async Task<List<FieldResponseContract>> GetApprovedFieldsAsync()
    {
        var fields = await _fieldRepo.GetApprovedFieldsAsync();
        return fields.Select(f => f.AsModel().AsContract()).ToList();
    }
}