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
    private readonly IFieldImageRepo _fieldImageRepo;

    public FieldService(IFieldRepo fieldRepo, IFieldImageRepo fieldImageRepo)
    {
        _fieldRepo = fieldRepo;
        _fieldImageRepo = fieldImageRepo;
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

    public async Task<FieldResponseContract> UploadFieldPhotoAsync(Guid fieldId, Guid ownerId, byte[] content, string contentType, string originalFileName)
    {
        var fieldEntity = await _fieldRepo.GetByIdAsync(fieldId);
        if (fieldEntity == null) throw new KeyNotFoundException();

        if (fieldEntity.OwnerId != ownerId)
            throw new ForbiddenException("Je kan enkel een foto uploaden voor je eigen terrein.");

        var ext = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

        var blobName = $"fields/{fieldId}/{Guid.NewGuid()}{ext}";
        var url = await _fieldImageRepo.UploadAsync(content, blobName, contentType);

        fieldEntity.ImageUrl = url;
        await _fieldRepo.UpdateAsync(fieldEntity);

        return fieldEntity.AsModel().AsContract();
    }
    public async Task<FieldResponseContract> UpdateFieldAsync(Guid fieldId, FieldUpdateContract update, Guid ownerId)
    {
        var field = await _fieldRepo.GetByIdAsync(fieldId);
        if (field == null) throw new KeyNotFoundException();

        if (field.OwnerId != ownerId)
            throw new ForbiddenException("Je kan enkel je eigen terrein aanpassen.");

        field.Name = update.Name;
        field.Description = update.Description;
        field.Capacity = update.Capacity;
        field.Address = update.Address;

        await _fieldRepo.UpdateAsync(field);

        return field.AsModel().AsContract();
    }
    public async Task DeleteFieldAsync(Guid id, Guid ownerId)
    {
        var field = await _fieldRepo.GetByIdAsync(id);
        if (field == null) throw new KeyNotFoundException();

        if (field.OwnerId != ownerId)
            throw new ForbiddenException("Je kan enkel je eigen terrein verwijderen.");

        await _fieldRepo.DeleteAsync(id);
    }
}