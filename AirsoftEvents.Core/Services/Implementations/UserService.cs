using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Domain.Enums;
using AirsoftEvents.Core.Persistence.Interfaces;
using AirsoftEvents.Core.Services.Interfaces;
using AirsoftEvents.Core.Services.Mapping;

namespace AirsoftEvents.Core.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepo _userRepo;

    public UserService(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<UserResponseContract> RegisterUserAsync(UserRequestContract request)
    {
        // Check: Bestaat email al?
        var existing = await _userRepo.GetByEmailAsync(request.Email);
        if (existing != null)
        {
            throw new ArgumentException("Email is al in gebruik.");
        }

        var user = request.AsModel();
        user.Id = Guid.NewGuid();
        user.Role = UserRole.Player;
        user.PasswordHash = request.Password; // Eigenlijk hashen, nu plain text voor demo

        var createdUser = await _userRepo.AddAsync(user);
        return createdUser.AsContract();
    }

    public async Task<UserResponseContract?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        return user?.AsContract();
    }
}