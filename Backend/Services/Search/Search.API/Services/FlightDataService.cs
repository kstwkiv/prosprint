using Search.API.Models;

namespace Search.API.Services;

public interface IFlightDataService
{
    IEnumerable<Flight> Search(string origin, string destination, DateTime date, int passengers);
    Flight? GetById(string flightId);
}

public class FlightDataService : IFlightDataService
{
    private static readonly List<Flight> _flights = Seed();

    public IEnumerable<Flight> Search(string origin, string destination, DateTime date, int passengers) =>
        _flights.Where(f =>
            f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
            f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
            f.DepartureTime.Date == date.Date &&
            f.AvailableSeats >= passengers &&
            f.Status != FlightStatus.Cancelled);

    public Flight? GetById(string flightId) =>
        _flights.FirstOrDefault(f => f.FlightId == flightId);

    private static List<Flight> Seed()
    {
        var d = DateTime.UtcNow.Date.AddDays(1);
        return
        [
            new() { FlightNumber="FB101", Airline="FlightBook Air", Origin="JFK", Destination="LHR",
                    DepartureTime=d.AddHours(8),  ArrivalTime=d.AddHours(16), TotalSeats=180, AvailableSeats=120, AircraftType="Boeing 737" },
            new() { FlightNumber="FB102", Airline="FlightBook Air", Origin="LHR", Destination="JFK",
                    DepartureTime=d.AddHours(10), ArrivalTime=d.AddHours(20), TotalSeats=180, AvailableSeats=95,  AircraftType="Boeing 737" },
            new() { FlightNumber="FB201", Airline="FlightBook Air", Origin="JFK", Destination="CDG",
                    DepartureTime=d.AddHours(9),  ArrivalTime=d.AddHours(17), TotalSeats=200, AvailableSeats=150, AircraftType="Airbus A320" },
            new() { FlightNumber="FB301", Airline="FlightBook Air", Origin="CDG", Destination="DXB",
                    DepartureTime=d.AddHours(14), ArrivalTime=d.AddHours(21), TotalSeats=250, AvailableSeats=60,  AircraftType="Airbus A380" },
        ];
    }
}
