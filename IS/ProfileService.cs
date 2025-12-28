using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IS.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IS;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

    public ProfileService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
{
    var userId = context.Subject.GetSubjectId();
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null) return;

    
    var principal = await _claimsFactory.CreateAsync(user);
    var claims = principal.Claims.ToList();

    
    var roles = await _userManager.GetRolesAsync(user);
    foreach (var role in roles)
    {
        
        claims.Add(new Claim(JwtClaimTypes.Role, role));
    }
    context.IssuedClaims.AddRange(claims);
}

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var userId = context.Subject.GetSubjectId();
        context.IsActive = await _userManager.FindByIdAsync(userId) is not null;
    }
}
