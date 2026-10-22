namespace Shared.Contracts.DTOs;

/// <summary>
/// Lightweight flight summary shared across Search, Fare and Booking services.
/// </summary>
public record FlightDto(
    string FlightId,
    string FlightNumber,
    string Airline,
    string Origin,
    string Destination,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    int AvailableSeats,
    string AircraftType,
    string Status
);
