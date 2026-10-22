using Identity.API.DTOs;
using Identity.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Constants;
using Shared.Models;

namespace Identity.API.Controllers;

/// <summary>User management — Admin role only.</summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = Roles.Admin)]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>List all users</summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserProfileResponse>>), 200)]
    public async Task<IActionResult> GetUsers()
    {
        var list = new List<UserProfileResponse>();
        foreach (var u in _userManager.Users.ToList())
        {
            var roles = await _userManager.GetRolesAsync(u);
            list.Add(new UserProfileResponse(u.Id, u.Email!, u.FirstName, u.LastName,
                u.PhoneNumber, u.CreatedAt, roles));
        }
        return Ok(ApiResponse<IEnumerable<UserProfileResponse>>.Ok(list));
    }

    /// <summary>Assign a role to a user</summary>
    [HttpPost("users/{userId}/roles")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> AssignRole(string userId, [FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(ApiResponse<object>.Fail("User not found."));

        if (!new[] { Roles.User, Roles.Admin, Roles.Manager }.Contains(request.Role))
            return BadRequest(ApiResponse<object>.Fail($"Invalid role. Allowed: {Roles.All}"));

        if (!await _roleManager.RoleExistsAsync(request.Role))
            await _roleManager.CreateAsync(new IdentityRole(request.Role));

        if (await _userManager.IsInRoleAsync(user, request.Role))
            return BadRequest(ApiResponse<object>.Fail($"User already has role '{request.Role}'."));

        var result = await _userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<object>.Fail("Role assignment failed.",
                result.Errors.Select(e => e.Description)));

        return Ok(ApiResponse<object>.Ok(new { }, $"Role '{request.Role}' assigned."));
    }

    /// <summary>Remove a role from a user</summary>
    [HttpDelete("users/{userId}/roles/{role}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(ApiResponse<object>.Fail("User not found."));

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<object>.Fail("Role removal failed.",
                result.Errors.Select(e => e.Description)));

        return NoContent();
    }

    /// <summary>Delete a user account</summary>
    [HttpDelete("users/{userId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(ApiResponse<object>.Fail("User not found."));

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<object>.Fail("Delete failed.",
                result.Errors.Select(e => e.Description)));

        return NoContent();
    }
}
