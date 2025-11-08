namespace RideShareApp.Contracts.Events;

public record PaymentProcessedEvent(
    Guid PaymentId,
    Guid RideId,
    Guid UserId,
    decimal Amount,
    string PaymentMethod,
    string Status,
    DateTime ProcessedAt
);

public record PaymentFailedEvent(
    Guid PaymentId,
    Guid RideId,
    Guid UserId,
    decimal Amount,
    string FailureReason,
    DateTime FailedAt
);


