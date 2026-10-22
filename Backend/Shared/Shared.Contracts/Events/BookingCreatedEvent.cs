namespace Shared.Contracts.Events;

/// <summary>
/// Integration event published when a booking is confirmed.
/// Consumed by Check-in Service.
/// </summary>
public record BookingCreatedEvent(
    string BookingId,
    string UserId,
    string FlightId,
    string FlightNumber,
    DateTime DepartureTime,
    int Passengers,
    DateTime OccurredAt
);
