using AirsoftEvents.Core.Domain.Models;

namespace AirsoftEvents.Core.Persistence.Interfaces;

public interface IFieldRepo
{
    Task<List<AirsoftField>> GetAllAsync();
    Task<AirsoftField?> GetByIdAsync(Guid id);
    Task<List<AirsoftField>> GetApprovedFieldsAsync();
    Task<AirsoftField> AddAsync(AirsoftField field);
    Task UpdateAsync(AirsoftField field);
    Task DeleteAsync(Guid id);
}