namespace Shared.Contracts.DTOs;

/// <summary>
/// Booking summary shared with the Check-in service.
/// </summary>
public record BookingDto(
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
