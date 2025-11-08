namespace RideShareApp.Contracts.Events;

public record RideRequestedEvent(
    Guid RideId,
    Guid RiderId,
    string PickupLocation,
    string DropoffLocation,
    DateTime RequestedAt
);

public record RideAcceptedEvent(
    Guid RideId,
    Guid RiderId,
    Guid DriverId,
    DateTime AcceptedAt
);

public record RideCompletedEvent(
    Guid RideId,
    Guid RiderId,
    Guid DriverId,
    decimal FareAmount,
    DateTime CompletedAt
);

public record RideCancelledEvent(
    Guid RideId,
    Guid CancelledByUserId,
    string Reason,
    DateTime CancelledAt
);


