using AirsoftEvents.Api.Contracts;

namespace AirsoftEvents.Domain.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseContract> RegisterUserAsync(UserRequestContract request);
    Task<UserResponseContract?> GetUserByIdAsync(Guid id);
}