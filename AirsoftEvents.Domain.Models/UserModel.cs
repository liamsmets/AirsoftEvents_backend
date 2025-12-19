using AirsoftEvents.Domain.Models.Enums;

namespace AirsoftEvents.Domain.Models;

public class UserModel
{
    public Guid Id{ get; set;}
    public string Username { get; set;} = string.Empty;
    public string Email{ get; set;} = string.Empty;
    public UserRole Role { get;set;}  = UserRole.Player;
    public string PasswordHash { get; set; } = string.Empty;
    public List<AirsoftFieldModel> Fields {get;set;} = new();
    public List<ReservationModel> Reservations {get;set;} = new();

}