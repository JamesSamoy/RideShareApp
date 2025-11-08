namespace RideShareApp.Contracts.Events;

public record NotificationSentEvent(
    Guid NotificationId,
    Guid UserId,
    string Message,
    string NotificationType,
    DateTime SentAt
);


