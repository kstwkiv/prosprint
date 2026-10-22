using System.ComponentModel.DataAnnotations;

namespace Search.API.DTOs;

public record FlightSearchRequest(
    [Required] string Origin,
    [Required] string Destination,
    [Required] DateTime DepartureDate,
    int Passengers = 1,
    string CabinClass = "Economy"
);

public record FlightSearchResponse(
    string FlightId,
    string FlightNumber,
    string Airline,
    string Origin,
    string Destination,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    TimeSpan Duration,
    int AvailableSeats,
    string AircraftType,
    string Status
);

public record AirportDto(string Code, string Name, string City, string Country);
