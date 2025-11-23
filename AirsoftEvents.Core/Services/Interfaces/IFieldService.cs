using AirsoftEvents.Core.Contracts;

namespace AirsoftEvents.Core.Services.Interfaces;

public interface IFieldService
{
    Task<FieldResponseContract> CreateFieldAsync(FieldRequestContract request);
    Task<List<FieldResponseContract>> GetAllFieldsAsync();
    Task<FieldResponseContract?> GetFieldByIdAsync(Guid id);
}