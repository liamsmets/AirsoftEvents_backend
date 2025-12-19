namespace AirsoftEvents.Api.Contracts;

public class UserRequestContract
{
    public string Username {get;set;} = string.Empty;
    public string Email {get;set;} = string.Empty;
    public string Password {get;set;} = string.Empty;
}