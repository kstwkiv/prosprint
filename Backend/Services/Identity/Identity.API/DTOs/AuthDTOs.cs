using System.ComponentModel.DataAnnotations;

namespace Identity.API.DTOs;

public record RegisterRequest(
    [Required] string FirstName,
    [Required] string LastName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password
);

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(
    string Token,
    string Email,
    string FullName,
    IList<string> Roles,
    DateTime ExpiresAt
);

public record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [Required][MinLength(6)] string NewPassword
);

public record AssignRoleRequest([Required] string Role);

public record UserProfileResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime CreatedAt,
    IList<string> Roles
);
