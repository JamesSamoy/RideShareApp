namespace RideShareApp.Contracts.Events;

public record UserCreatedEvent(
    Guid UserId,
    string Name,
    string Email,
    string PhoneNumber,
    DateTime CreatedAt
);

public record UserUpdatedEvent(
    Guid UserId,
    string? Name,
    string? Email,
    string? PhoneNumber,
    DateTime UpdatedAt
);


