namespace Shared.Contracts.DTOs;

/// <summary>
/// Minimal user info that other services may receive from the gateway JWT claims.
/// </summary>
public record UserDto(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    IList<string> Roles
);
