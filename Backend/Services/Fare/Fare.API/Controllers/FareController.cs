using Fare.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Fare.API.Controllers;

[ApiController]
[Route("api/fares")]
[Authorize]
public class FareController : ControllerBase
{
    private static readonly Dictionary<string, decimal> RatePerHour = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Economy"]  = 50m,
        ["Business"] = 180m,
        ["First"]    = 350m
    };

    private const decimal TaxRate       = 0.12m;
    private const decimal FuelSurcharge = 25m;
    private const decimal AirportFee    = 15m;

    /// <summary>Calculate fare for a flight, cabin class and passenger count</summary>
    [HttpGet("calculate")]
    [ProducesResponseType(typeof(ApiResponse<FareResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public IActionResult Calculate([FromQuery] FareRequest request)
    {
        if (!RatePerHour.TryGetValue(request.CabinClass, out var rate))
            return BadRequest(ApiResponse<object>.Fail(
                $"Invalid cabin class. Supported: {string.Join(", ", RatePerHour.Keys)}"));

        var hours   = EstimateDuration(request.FlightId);
        var base1   = Math.Round(rate * (decimal)hours, 2);
        var taxes   = Math.Round(base1 * TaxRate, 2);
        var perPax  = Math.Round(base1 + taxes + FuelSurcharge + AirportFee, 2);
        var total   = Math.Round(perPax * request.Passengers, 2);

        var breakdown = new FareBreakdown(base1, taxes, FuelSurcharge, AirportFee, perPax, total, "USD");
        return Ok(ApiResponse<FareResponse>.Ok(
            new FareResponse(request.FlightId, request.CabinClass, request.Passengers, breakdown)));
    }

    /// <summary>Get available cabin classes and indicative prices for a flight</summary>
    [HttpGet("{flightId}/classes")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CabinClassFare>>), 200)]
    public IActionResult GetCabinClasses(string flightId)
    {
        var hours = EstimateDuration(flightId);
        var fares = RatePerHour.Select(kv =>
        {
            var b = kv.Value * (decimal)hours;
            var t = Math.Round(b * TaxRate, 2);
            var p = Math.Round(b + t + FuelSurcharge + AirportFee, 2);
            return new CabinClassFare(kv.Key, p, "USD",
                kv.Key == "Economy"  ? "Standard seat, carry-on included" :
                kv.Key == "Business" ? "Extra legroom + complimentary meal" :
                                       "Full luxury suite experience");
        });
        return Ok(ApiResponse<IEnumerable<CabinClassFare>>.Ok(fares));
    }

    // Stub — in production derive from actual flight schedule data
    private static double EstimateDuration(string flightId) =>
        Math.Abs(flightId.GetHashCode()) % 10 + 3;
}
