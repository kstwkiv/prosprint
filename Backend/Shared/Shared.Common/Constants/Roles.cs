namespace Shared.Common.Constants;

public static class Roles
{
    public const string Admin   = "Admin";
    public const string Manager = "Manager";
    public const string User    = "User";

    /// <summary>Comma-separated string for use with [Authorize(Roles = ...)]</summary>
    public const string AdminOrManager = $"{Admin},{Manager}";
    public const string All            = $"{Admin},{Manager},{User}";
}
