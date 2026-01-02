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
    Task<FieldResponseContract> UploadFieldPhotoAsync(Guid fieldId, Guid ownerId,bool isAdmin, byte[] content, string contentType, string originalFileName);
    Task<FieldResponseContract> UpdateFieldAsync(Guid fieldId,FieldUpdateContract update, Guid ownerId, bool isAdmin);
    Task DeleteFieldAsync(Guid id, Guid ownerId,bool isAdmin);
    Task RejectFieldAsync(Guid id);

}