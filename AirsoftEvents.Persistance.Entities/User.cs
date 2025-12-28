using AirsoftEvents.Domain.Models.Enums;

namespace AirsoftEvents.Persistance.Entities;

public class User
{
    public Guid Id {get;set;}
    public string Username {get;set;} = string.Empty;
    public string Email {get;set;} = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public required UserRole Role {get;set;} = UserRole.Player;
}