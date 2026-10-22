using System.ComponentModel.DataAnnotations;

namespace Checkin.API.DTOs;

public record CheckInRequest(
    [Required] string BookingId,
    [Required] string FlightId,
    [Required] string PassengerName,
    string? PreferredSeat = null
);

public record CheckInResponse(
    string CheckInId,
    string BookingId,
    string FlightId,
    string PassengerName,
    string SeatNumber,
    string BoardingPass,
    DateTime CheckInTime,
    string Status
);

public record UpdateBoardingStatusRequest(
    [Required] string CheckInId,
    [Required] string Status
);
