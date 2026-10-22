using Identity.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Constants;
using Shared.Models;

namespace Identity.API.Controllers;

/// <summary>Read-only user views for Manager and Admin roles.</summary>
[ApiController]
[Route("api/manager")]
[Authorize(Roles = Roles.AdminOrManager)]
public class ManagerController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ManagerController(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    /// <summary>List all regular (User-role) accounts</summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
    public async Task<IActionResult> GetRegularUsers()
    {
        var result = new List<object>();
        foreach (var u in _userManager.Users.ToList())
        {
            var roles = await _userManager.GetRolesAsync(u);
            if (roles.Contains(Roles.User))
                result.Add(new { u.Id, u.Email, u.FirstName, u.LastName, u.CreatedAt });
        }
        return Ok(ApiResponse<IEnumerable<object>>.Ok(result));
    }
}
