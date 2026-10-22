using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Search.API.DTOs;
using Search.API.Services;
using Shared.Models;

namespace Search.API.Controllers;

[ApiController]
[Route("api/search")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly IFlightDataService _flights;
    public SearchController(IFlightDataService flights) => _flights = flights;

    /// <summary>Search available flights</summary>
    [HttpGet("flights")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FlightSearchResponse>>), 200)]
    public IActionResult SearchFlights([FromQuery] FlightSearchRequest request)
    {
        var results = _flights
            .Search(request.Origin, request.Destination, request.DepartureDate, request.Passengers)
            .Select(f => new FlightSearchResponse(
                f.FlightId, f.FlightNumber, f.Airline,
                f.Origin, f.Destination,
                f.DepartureTime, f.ArrivalTime,
                f.ArrivalTime - f.DepartureTime,
                f.AvailableSeats, f.AircraftType,
                f.Status.ToString()));

        return Ok(ApiResponse<IEnumerable<FlightSearchResponse>>.Ok(results));
    }

    /// <summary>Get a specific flight by ID</summary>
    [HttpGet("flights/{flightId}")]
    [ProducesResponseType(typeof(ApiResponse<FlightSearchResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public IActionResult GetFlight(string flightId)
    {
        var f = _flights.GetById(flightId);
        if (f is null) return NotFound(ApiResponse<object>.Fail($"Flight '{flightId}' not found."));

        return Ok(ApiResponse<FlightSearchResponse>.Ok(new FlightSearchResponse(
            f.FlightId, f.FlightNumber, f.Airline,
            f.Origin, f.Destination,
            f.DepartureTime, f.ArrivalTime,
            f.ArrivalTime - f.DepartureTime,
            f.AvailableSeats, f.AircraftType,
            f.Status.ToString())));
    }

    /// <summary>List supported airports — public</summary>
    [HttpGet("airports")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AirportDto>>), 200)]
    public IActionResult GetAirports() =>
        Ok(ApiResponse<IEnumerable<AirportDto>>.Ok(new AirportDto[]
        {
            new("JFK", "John F. Kennedy International", "New York",  "USA"),
            new("LHR", "Heathrow",                      "London",    "UK"),
            new("CDG", "Charles de Gaulle",              "Paris",     "France"),
            new("DXB", "Dubai International",            "Dubai",     "UAE"),
            new("SIN", "Changi",                         "Singapore", "Singapore"),
        }));
}
