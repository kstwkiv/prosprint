using Identity.API.DTOs;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Constants;
using Shared.Common.Extensions;
using Shared.Models;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService)
    {
        _userManager  = userManager;
        _signInManager = signInManager;
        _roleManager  = roleManager;
        _tokenService = tokenService;
    }

    /// <summary>Register a new user — assigned User role automatically</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            return Conflict(ApiResponse<object>.Fail("Email is already in use."));

        var user = new ApplicationUser
        {
            UserName  = request.Email,
            Email     = request.Email,
            FirstName = request.FirstName,
            LastName  = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<object>.Fail("Registration failed.",
                result.Errors.Select(e => e.Description)));

        await EnsureRoleAsync(Roles.User);
        await _userManager.AddToRoleAsync(user, Roles.User);

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _tokenService.GenerateToken(user, roles);

        return StatusCode(201, ApiResponse<AuthResponse>.Ok(
            new AuthResponse(token, user.Email!, $"{user.FirstName} {user.LastName}", roles, expiresAt),
            "Registration successful."));
    }

    /// <summary>Login and receive a JWT bearer token</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(ApiResponse<object>.Fail("Invalid credentials."));

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return StatusCode(423, ApiResponse<object>.Fail("Account is temporarily locked out."));
            return Unauthorized(ApiResponse<object>.Fail("Invalid credentials."));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _tokenService.GenerateToken(user, roles);

        return Ok(ApiResponse<AuthResponse>.Ok(
            new AuthResponse(token, user.Email!, $"{user.FirstName} {user.LastName}", roles, expiresAt)));
    }

    /// <summary>Get current authenticated user's profile</summary>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), 200)]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound(ApiResponse<object>.Fail("User not found."));

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(ApiResponse<UserProfileResponse>.Ok(
            new UserProfileResponse(user.Id, user.Email!, user.FirstName, user.LastName,
                user.PhoneNumber, user.CreatedAt, roles)));
    }

    /// <summary>Change password for the currently authenticated user</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound(ApiResponse<object>.Fail("User not found."));

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<object>.Fail("Password change failed.",
                result.Errors.Select(e => e.Description)));

        return NoContent();
    }

    private async Task EnsureRoleAsync(string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));
    }
}
