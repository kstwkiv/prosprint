using System.ComponentModel.DataAnnotations;

namespace Fare.API.DTOs;

public record FareRequest(
    [Required] string FlightId,
    string CabinClass = "Economy",
    [Range(1, 9)] int Passengers = 1
);

public record FareBreakdown(
    decimal BaseFare,
    decimal Taxes,
    decimal FuelSurcharge,
    decimal AirportFee,
    decimal TotalPerPassenger,
    decimal TotalAmount,
    string Currency
);

public record FareResponse(
    string FlightId,
    string CabinClass,
    int Passengers,
    FareBreakdown Breakdown
);

public record CabinClassFare(
    string Class,
    decimal Price,
    string Currency,
    string Description
);
