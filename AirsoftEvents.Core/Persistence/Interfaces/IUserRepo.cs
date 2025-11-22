using AirsoftEvents.Core.Domain.Models;

namespace AirsoftEvents.Core.Persistence.Interfaces;

public interface IUserRepo
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}