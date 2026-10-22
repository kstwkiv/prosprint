namespace Search.API.Models;

public class Flight
{
    public string FlightId      { get; set; } = Guid.NewGuid().ToString();
    public string FlightNumber  { get; set; } = string.Empty;
    public string Airline       { get; set; } = string.Empty;
    public string Origin        { get; set; } = string.Empty;
    public string Destination   { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime   { get; set; }
    public int TotalSeats       { get; set; }
    public int AvailableSeats   { get; set; }
    public string AircraftType  { get; set; } = string.Empty;
    public FlightStatus Status  { get; set; } = FlightStatus.Scheduled;
}

public enum FlightStatus { Scheduled, Boarding, Departed, Arrived, Cancelled, Delayed }
