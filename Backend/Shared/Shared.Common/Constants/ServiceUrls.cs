namespace Shared.Common.Constants;

/// <summary>
/// Default localhost base URLs per service.
/// Override via environment variables or appsettings in deployed environments.
/// </summary>
public static class ServiceUrls
{
    public const string Gateway         = "http://localhost:5000";
    public const string IdentityApi     = "http://localhost:5001";
    public const string SearchApi       = "http://localhost:5002";
    public const string FareApi         = "http://localhost:5003";
    public const string BookingApi      = "http://localhost:5004";
    public const string CheckinApi      = "http://localhost:5005";
}
