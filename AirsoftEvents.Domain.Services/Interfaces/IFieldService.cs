using AirsoftEvents.Api.Contracts;

namespace AirsoftEvents.Domain.Services.Interfaces;

public interface IFieldService
{
    Task<FieldResponseContract> CreateFieldAsync(FieldRequestContract request, Guid ownerId);
    Task<List<FieldResponseContract>> GetAllFieldsAsync();
    Task<FieldResponseContract?> GetFieldByIdAsync(Guid id);
    Task<List<FieldResponseContract>> GetFieldByOwnerIdAsync(Guid id);
    Task<List<FieldResponseContract>> GetApprovedFieldsAsync();
    Task ApproveFieldAsync(Guid id);
}