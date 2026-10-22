using Booking.API.Data;
using Booking.API.DTOs;
using Booking.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Constants;
using Shared.Common.Extensions;
using Shared.Models;

namespace Booking.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly BookingDbContext _db;
    public BookingController(BookingDbContext db) => _db = db;

    /// <summary>Create a new booking</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookingResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        if (request.Passengers is null || request.Passengers.Count == 0)
            return BadRequest(ApiResponse<object>.Fail("At least one passenger is required."));

        var userId = User.GetUserId();
        var rng = new Random();

        var passengers = request.Passengers.Select((p, i) => new Passenger
        {
            FirstName      = p.FirstName,
            LastName       = p.LastName,
            PassportNumber = p.PassportNumber,
            Nationality    = p.Nationality,
            DateOfBirth    = p.DateOfBirth,
            SeatNumber     = $"{rng.Next(1, 35)}{(char)('A' + i % 6)}"
        }).ToList();

        var booking = new BookingRecord
        {
            UserId        = userId,
            FlightId      = request.FlightId,
            FlightNumber  = request.FlightNumber,
            Origin        = request.Origin,
            Destination   = request.Destination,
            DepartureTime = request.DepartureTime,
            CabinClass    = request.CabinClass,
            Passengers    = request.Passengers.Count,
            TotalAmount   = 450m * request.Passengers.Count, // stub — call Fare.API in production
            PassengerDetails = passengers
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return StatusCode(201, ApiResponse<BookingResponse>.Ok(Map(booking), "Booking confirmed."));
    }

    /// <summary>Get current user's bookings</summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookingResponse>>), 200)]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = User.GetUserId();
        var list = await _db.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookedAt)
            .ToListAsync();
        return Ok(ApiResponse<IEnumerable<BookingResponse>>.Ok(list.Select(Map)));
    }

    /// <summary>Get booking by ID</summary>
    [HttpGet("{bookingId}")]
    [ProducesResponseType(typeof(ApiResponse<BookingResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetBooking(string bookingId)
    {
        var booking = await _db.Bookings.Include(b => b.PassengerDetails)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        if (booking is null)
            return NotFound(ApiResponse<object>.Fail($"Booking '{bookingId}' not found."));

        var userId = User.GetUserId();
        if (booking.UserId != userId && !User.IsInRole(Roles.Admin) && !User.IsInRole(Roles.Manager))
            return Forbid();

        return Ok(ApiResponse<BookingResponse>.Ok(Map(booking)));
    }

    /// <summary>Cancel a booking</summary>
    [HttpDelete("{bookingId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> CancelBooking(string bookingId)
    {
        var booking = await _db.Bookings.FindAsync(bookingId);
        if (booking is null)
            return NotFound(ApiResponse<object>.Fail($"Booking '{bookingId}' not found."));

        var userId = User.GetUserId();
        if (booking.UserId != userId && !User.IsInRole(Roles.Admin) && !User.IsInRole(Roles.Manager))
            return Forbid();

        if (booking.Status == BookingStatus.Cancelled)
            return BadRequest(ApiResponse<object>.Fail("Booking is already cancelled."));

        booking.Status = BookingStatus.Cancelled;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>List all bookings — Admin/Manager only</summary>
    [HttpGet("all")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(PagedResult<BookingResponse>), 200)]
    public async Task<IActionResult> GetAllBookings([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var total = await _db.Bookings.CountAsync();
        var items = await _db.Bookings
            .OrderByDescending(b => b.BookedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PagedResult<BookingResponse>
        {
            Items = items.Select(Map),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    private static BookingResponse Map(BookingRecord b) => new(
        b.BookingId, b.FlightId, b.FlightNumber,
        b.Origin, b.Destination, b.DepartureTime,
        b.CabinClass, b.Passengers, b.TotalAmount,
        b.Currency, b.Status.ToString(), b.BookedAt);
}
