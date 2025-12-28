using System.Net;
using AirsoftEvents.Api.Contracts;
using AirsoftEvents.Domain.Models;
using AirsoftEvents.Persistance.Entities;

namespace AirsoftEvents.Domain.Services.Mapping;

public static class UserMappingExtensions
{
    public static UserModel AsModel(this UserRequestContract request)
    {
        return new UserModel
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = request.Password
        };
    }

    public static User AsEntity(this UserModel userModel)
    {
        return new User
        {
            Id = userModel.Id,
            Username = userModel.Username,
            Email = userModel.Email,
            PasswordHash = userModel.PasswordHash,
            Role = userModel.Role
        };
    }

    public static UserResponseContract AsContract(this UserModel model)
    {
        return new UserResponseContract
        {
            Id = model.Id,
            Username = model.Username,
            Email = model.Email,
            Role = model.Role
        };
    }

    public static UserModel AsModel(this User userEntity)
    {
        return new UserModel
        {
          Id = userEntity.Id,
          Username = userEntity.Username,
          Email = userEntity.Email,
          Role = userEntity.Role,
          PasswordHash = userEntity.PasswordHash
        };
    }
}