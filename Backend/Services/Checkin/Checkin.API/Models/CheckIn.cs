namespace Checkin.API.Models;

public class CheckIn
{
    public string CheckInId     { get; set; } = Guid.NewGuid().ToString();
    public string BookingId     { get; set; } = string.Empty;
    public string FlightId      { get; set; } = string.Empty;
    public string UserId        { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string SeatNumber    { get; set; } = string.Empty;
    public string BoardingPass  { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public CheckInStatus Status { get; set; } = CheckInStatus.CheckedIn;
}

public enum CheckInStatus { CheckedIn, Boarded, NoShow, Cancelled }
