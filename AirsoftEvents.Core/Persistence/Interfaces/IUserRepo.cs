using AirsoftEvents.Core.Domain.Models;

namespace AirsoftEvents.Core.Persistence.Interfaces;

public interface IUserRepo
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailasync(string email);
    Task<User> addAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}