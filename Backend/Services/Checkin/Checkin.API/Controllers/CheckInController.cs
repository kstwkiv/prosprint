using Checkin.API.Data;
using Checkin.API.DTOs;
using Checkin.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Constants;
using Shared.Common.Extensions;
using Shared.Models;

namespace Checkin.API.Controllers;

[ApiController]
[Route("api/checkin")]
[Authorize]
public class CheckInController : ControllerBase
{
    private readonly CheckinDbContext _db;
    public CheckInController(CheckinDbContext db) => _db = db;

    /// <summary>Check in for a flight</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CheckInResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
    {
        var already = await _db.CheckIns
            .AnyAsync(c => c.BookingId == request.BookingId && c.PassengerName == request.PassengerName);

        if (already)
            return BadRequest(ApiResponse<object>.Fail("Passenger already checked in for this booking."));

        var seat        = request.PreferredSeat ?? $"{new Random().Next(1, 35)}{(char)('A' + new Random().Next(0, 6))}";
        var boardingPass = $"BP-{request.FlightId[..Math.Min(6, request.FlightId.Length)].ToUpper()}-{Guid.NewGuid():N}"[..20].ToUpper();

        var checkIn = new CheckIn
        {
            BookingId     = request.BookingId,
            FlightId      = request.FlightId,
            UserId        = User.GetUserId(),
            PassengerName = request.PassengerName,
            SeatNumber    = seat,
            BoardingPass  = boardingPass
        };

        _db.CheckIns.Add(checkIn);
        await _db.SaveChangesAsync();

        return StatusCode(201, ApiResponse<CheckInResponse>.Ok(Map(checkIn), "Check-in successful."));
    }

    /// <summary>Get check-in by ID</summary>
    [HttpGet("{checkInId}")]
    [ProducesResponseType(typeof(ApiResponse<CheckInResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetCheckIn(string checkInId)
    {
        var c = await _db.CheckIns.FindAsync(checkInId);
        if (c is null) return NotFound(ApiResponse<object>.Fail($"Check-in '{checkInId}' not found."));

        var userId = User.GetUserId();
        if (c.UserId != userId && !User.IsInRole(Roles.Admin) && !User.IsInRole(Roles.Manager))
            return Forbid();

        return Ok(ApiResponse<CheckInResponse>.Ok(Map(c)));
    }

    /// <summary>Get all check-ins for a booking</summary>
    [HttpGet("booking/{bookingId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CheckInResponse>>), 200)]
    public async Task<IActionResult> GetByBooking(string bookingId)
    {
        var userId = User.GetUserId();
        var isPriv = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Manager);

        var q = _db.CheckIns.Where(c => c.BookingId == bookingId);
        if (!isPriv) q = q.Where(c => c.UserId == userId);

        return Ok(ApiResponse<IEnumerable<CheckInResponse>>.Ok(
            (await q.ToListAsync()).Select(Map)));
    }

    /// <summary>Update boarding status — Admin/Manager only</summary>
    [HttpPut("status")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<CheckInResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateBoardingStatusRequest request)
    {
        var c = await _db.CheckIns.FindAsync(request.CheckInId);
        if (c is null) return NotFound(ApiResponse<object>.Fail($"Check-in '{request.CheckInId}' not found."));

        if (!Enum.TryParse<CheckInStatus>(request.Status, ignoreCase: true, out var status))
            return BadRequest(ApiResponse<object>.Fail(
                $"Invalid status. Allowed: {string.Join(", ", Enum.GetNames<CheckInStatus>())}"));

        c.Status = status;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<CheckInResponse>.Ok(Map(c), "Status updated."));
    }

    /// <summary>List all check-ins — Admin/Manager only</summary>
    [HttpGet("all")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CheckInResponse>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] string? flightId = null)
    {
        var q = _db.CheckIns.AsQueryable();
        if (!string.IsNullOrEmpty(flightId)) q = q.Where(c => c.FlightId == flightId);

        return Ok(ApiResponse<IEnumerable<CheckInResponse>>.Ok(
            (await q.OrderByDescending(c => c.CheckInTime).ToListAsync()).Select(Map)));
    }

    private static CheckInResponse Map(CheckIn c) => new(
        c.CheckInId, c.BookingId, c.FlightId,
        c.PassengerName, c.SeatNumber, c.BoardingPass,
        c.CheckInTime, c.Status.ToString());
}
