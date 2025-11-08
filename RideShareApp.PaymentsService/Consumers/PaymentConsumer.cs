using MassTransit;
using RideShareApp.Contracts.Events;

namespace RideShareApp.PaymentsService.Consumers;

public class PaymentConsumer : IConsumer<RideCompletedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentConsumer> _logger;

    public PaymentConsumer(IPublishEndpoint publishEndpoint, ILogger<PaymentConsumer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RideCompletedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation(
            "Payment Service: Processing payment for Ride {RideId}, Amount: {FareAmount:C}",
            @event.RideId, @event.FareAmount);

        // Simulate payment processing
        var paymentId = Guid.NewGuid();
        var paymentSuccessful = SimulatePaymentProcessing(@event.FareAmount);

        if (paymentSuccessful)
        {
            var paymentProcessedEvent = new PaymentProcessedEvent(
                paymentId,
                @event.RideId,
                @event.RiderId,
                @event.FareAmount,
                "CreditCard",
                "Completed",
                DateTime.UtcNow
            );

            await _publishEndpoint.Publish(paymentProcessedEvent);
            _logger.LogInformation("Payment {PaymentId} processed successfully for Ride {RideId}", paymentId, @event.RideId);
        }
        else
        {
            var paymentFailedEvent = new PaymentFailedEvent(
                paymentId,
                @event.RideId,
                @event.RiderId,
                @event.FareAmount,
                "Payment processing failed - insufficient funds or card declined",
                DateTime.UtcNow
            );

            await _publishEndpoint.Publish(paymentFailedEvent);
            _logger.LogWarning("Payment {PaymentId} failed for Ride {RideId}", paymentId, @event.RideId);
        }
    }

    private bool SimulatePaymentProcessing(decimal amount)
    {
        // Simulate payment processing - in real app, this would call a payment gateway
        // For demo purposes, we'll randomly fail 5% of payments
        return Random.Shared.Next(100) > 5;
    }
}


