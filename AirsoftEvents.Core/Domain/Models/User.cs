using AirsoftEvents.Core.Domain.Enums;

namespace AirsoftEvents.Core.Domain.Models;

public class User
{
    public Guid Id{ get; set;}
    public string Username { get; set;} = string.Empty;
    public string Email{ get; set;} = string.Empty;
    public UserRole Role { get;set;}  = UserRole.Player;
    public string PasswordHash { get; set; } = string.Empty;
    public List<AirsoftField> Fields {get;set;} = new();
    public List<Reservation> Reservations {get;set;} = new();

}