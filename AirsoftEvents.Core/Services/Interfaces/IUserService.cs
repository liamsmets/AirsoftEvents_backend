using AirsoftEvents.Core.Contracts;

namespace AirsoftEvents.Core.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseContract> RegisterUserAsync(UserRequestContract request);
    Task<UserResponseContract?> GetUserByIdAsync(Guid id);
}