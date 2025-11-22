using AirsoftEvents.Core.Contracts;
using AirsoftEvents.Core.Domain.Models;

namespace AirsoftEvents.Core.Services.Mapping;

public static class UserMappingExtensions
{
    public static User AsModel(this UserRequestContract request)
    {
        return new User
        {
            Username = request.Username,
            Email = request.Email
        };
    }

    public static UserResponseContract AsContract(this User model)
    {
        return new UserResponseContract
        {
            Id = model.Id,
            Username = model.Username,
            Email = model.Email,
            Role = model.Role.ToString()
        };
    }
}