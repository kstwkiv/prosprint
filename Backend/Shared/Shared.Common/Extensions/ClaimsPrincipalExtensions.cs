using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Shared.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Returns the user ID from Sub or NameIdentifier claim.</summary>
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? string.Empty;

    /// <summary>Returns the user's email claim.</summary>
    public static string GetEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Email)
        ?? user.FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? string.Empty;
}
