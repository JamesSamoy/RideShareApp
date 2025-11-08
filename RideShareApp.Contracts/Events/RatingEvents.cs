namespace RideShareApp.Contracts.Events;

public record RatingSubmittedEvent(
    Guid RatingId,
    Guid RideId,
    Guid RatedByUserId,
    Guid RatedUserId,
    int Rating,
    string? Comment,
    DateTime SubmittedAt
);


