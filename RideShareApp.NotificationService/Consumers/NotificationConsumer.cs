using MassTransit;
using RideShareApp.Contracts.Events;

namespace RideShareApp.NotificationService.Consumers;

public class NotificationConsumer : IConsumer<UserCreatedEvent>,
                                    IConsumer<RideRequestedEvent>,
                                    IConsumer<RideAcceptedEvent>,
                                    IConsumer<RideCompletedEvent>,
                                    IConsumer<RideCancelledEvent>
{
    private readonly ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(ILogger<NotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation(
            "Notification: User {UserId} ({Name}) created successfully. Sending welcome email.",
            @event.UserId, @event.Name);

        // In a real application, you would send an email/SMS/push notification here
        await Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<RideRequestedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation(
            "Notification: Ride {RideId} requested by Rider {RiderId}. Searching for available drivers...",
            @event.RideId, @event.RiderId);

        // Send notification to nearby drivers
        await Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<RideAcceptedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation(
            "Notification: Ride {RideId} accepted by Driver {DriverId}. Notifying rider {RiderId}.",
            @event.RideId, @event.DriverId, @event.RiderId);

        // Send notification to rider that driver has accepted
        await Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<RideCompletedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation(
            "Notification: Ride {RideId} completed. Fare: {FareAmount:C}. Notifying rider {RiderId} and driver {DriverId}.",
            @event.RideId, @event.FareAmount, @event.RiderId, @event.DriverId);

        // Send completion notifications and receipt
        await Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<RideCancelledEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation(
            "Notification: Ride {RideId} cancelled by {CancelledByUserId}. Reason: {Reason}.",
            @event.RideId, @event.CancelledByUserId, @event.Reason);

        // Send cancellation notification to affected parties
        await Task.CompletedTask;
    }
}


