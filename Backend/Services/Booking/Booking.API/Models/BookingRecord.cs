namespace Booking.API.Models;

public class BookingRecord
{
    public string BookingId     { get; set; } = Guid.NewGuid().ToString();
    public string UserId        { get; set; } = string.Empty;
    public string FlightId      { get; set; } = string.Empty;
    public string FlightNumber  { get; set; } = string.Empty;
    public string Origin        { get; set; } = string.Empty;
    public string Destination   { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public string CabinClass    { get; set; } = "Economy";
    public int Passengers       { get; set; }
    public decimal TotalAmount  { get; set; }
    public string Currency      { get; set; } = "USD";
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public DateTime BookedAt    { get; set; } = DateTime.UtcNow;
    public List<Passenger> PassengerDetails { get; set; } = new();
}

public class Passenger
{
    public string FirstName      { get; set; } = string.Empty;
    public string LastName       { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string Nationality    { get; set; } = string.Empty;
    public DateTime DateOfBirth  { get; set; }
    public string SeatNumber     { get; set; } = string.Empty;
}

public enum BookingStatus { Confirmed, Cancelled, CheckedIn, Completed }
