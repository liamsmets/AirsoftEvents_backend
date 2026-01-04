using AirsoftEvents.Domain.Models;
using AirsoftEvents.Domain.Models.Enums;
using AirsoftEvents.Persistance.Entities;
namespace AirsoftEvents.Persistance.Interface;

public interface IFieldRepo
{
    Task<List<Field>> GetAllAsync();
    Task<Field?> GetByIdAsync(Guid id);
    Task<List<Field>> GetByOwnerId(Guid id);
    Task<List<Field>> GetApprovedFieldsAsync();
    Task<List<Field>> GetApprovedFieldsByIdAsync(Guid ownerId, FieldStatus status);
    Task<Field> AddAsync(Field field);
    Task UpdateAsync(Field field);
    Task DeleteAsync(Guid id);
}