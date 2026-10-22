using System.ComponentModel.DataAnnotations;

namespace Booking.API.DTOs;

public record CreateBookingRequest(
    [Required] string FlightId,
    [Required] string FlightNumber,
    [Required] string Origin,
    [Required] string Destination,
    [Required] DateTime DepartureTime,
    string CabinClass = "Economy",
    [Required][MinLength(1)] List<PassengerDto>? Passengers = null
);

public record PassengerDto(
    [Required] string FirstName,
    [Required] string LastName,
    [Required] string PassportNumber,
    [Required] string Nationality,
    [Required] DateTime DateOfBirth
);

public record BookingResponse(
    string BookingId,
    string FlightId,
    string FlightNumber,
    string Origin,
    string Destination,
    DateTime DepartureTime,
    string CabinClass,
    int Passengers,
    decimal TotalAmount,
    string Currency,
    string Status,
    DateTime BookedAt
);
