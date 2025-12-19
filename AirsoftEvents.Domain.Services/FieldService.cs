using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Models.Enums;
using AirsoftEvents.Persistance.Interface;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Domain.Services.Mapping;

namespace AirsoftEvents.Domain.Services;

public class FieldService : IFieldService
{
    private readonly IFieldRepo _fieldRepo;

    public FieldService(IFieldRepo fieldRepo)
    {
        _fieldRepo = fieldRepo;
    }

    public async Task<FieldResponseContract> CreateFieldAsync(FieldRequestContract request)
    {
        var field = request.AsModel();
        field.Id = Guid.NewGuid();
        field.Status = FieldStatus.Pending;

        var createdField = await _fieldRepo.AddAsync(field.AsEntity());
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
}