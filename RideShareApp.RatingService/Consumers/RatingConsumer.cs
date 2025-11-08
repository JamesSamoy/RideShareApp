using MassTransit;
using RideShareApp.Contracts.Events;

namespace RideShareApp.RatingService.Consumers;

public class RatingConsumer : IConsumer<RideCompletedEvent>
{
    private readonly ILogger<RatingConsumer> _logger;

    public RatingConsumer(ILogger<RatingConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RideCompletedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation(
            "Rating Service: Ride {RideId} completed. Ready to accept ratings from rider {RiderId} and driver {DriverId}.",
            @event.RideId, @event.RiderId, @event.DriverId);

        // In a real application, you would:
        // 1. Create rating entries in the database
        // 2. Send notification to both rider and driver to submit their ratings
        // 3. Wait for RatingSubmittedEvent to process

        await Task.CompletedTask;
    }
}


