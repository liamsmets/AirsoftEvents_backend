using System.Security.Claims;

namespace AirsoftEvents.Api.Extensions;

public static class UserExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub") 
                  ?? throw new UnauthorizedAccessException("No 'sub' claim found in token.");

        if (Guid.TryParse(sub, out var guid))
        {
            return guid;
        }
        return Guid.Empty;
    }
}