namespace Shared.Contracts.Events;

/// <summary>
/// Integration event published when a booking is cancelled.
/// </summary>
public record BookingCancelledEvent(
    string BookingId,
    string UserId,
    string FlightId,
    DateTime OccurredAt
);
