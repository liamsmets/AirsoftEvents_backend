using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Domain.Enums;
using AirsoftEvents.Core.Persistence.Interfaces;
using AirsoftEvents.Core.Services.Interfaces;
using AirsoftEvents.Core.Services.Mapping;

namespace AirsoftEvents.Core.Services.Implementations;

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

        var createdField = await _fieldRepo.AddAsync(field);
        return createdField.AsContract();
    }

    public async Task<List<FieldResponseContract>> GetAllFieldsAsync()
    {
        var fields = await _fieldRepo.GetAllAsync();
        return fields.Select(f => f.AsContract()).ToList();
    }
}