using Microsoft.AspNetCore.Mvc;
using RideShareApp.Contracts.Events;
using MassTransit;

namespace RideShareApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RiderController(IPublishEndpoint publishEndpoint, ILogger<RiderController> logger)
    : ControllerBase
{
    [HttpPost("request-ride")]
    public async Task<IActionResult> RequestRide([FromBody] RequestRideRequest request)
    {
        var rideId = Guid.NewGuid();
        var @event = new RideRequestedEvent(
            rideId,
            request.RiderId,
            request.PickupLocation,
            request.DropoffLocation,
            DateTime.UtcNow
        );

        await publishEndpoint.Publish(@event);

        logger.LogInformation("Published RideRequestedEvent for RideId: {RideId}, RiderId: {RiderId}", rideId, request.RiderId);

        return Ok(new { RideId = rideId, Message = "Ride requested successfully" });
    }

    [HttpPost("{rideId}/cancel")]
    public async Task<IActionResult> CancelRide(Guid rideId, [FromBody] CancelRideRequest request)
    {
        var @event = new RideCancelledEvent(
            rideId,
            request.CancelledByUserId,
            request.Reason,
            DateTime.UtcNow
        );

        await publishEndpoint.Publish(@event);

        logger.LogInformation("Published RideCancelledEvent for RideId: {RideId}", rideId);

        return Ok(new { Message = "Ride cancelled successfully" });
    }

    [HttpPost("{rideId}/complete")]
    public async Task<IActionResult> CompleteRide(Guid rideId, [FromBody] CompleteRideRequest request)
    {
        var @event = new RideCompletedEvent(
            rideId,
            request.RiderId,
            request.DriverId,
            request.FareAmount,
            DateTime.UtcNow
        );

        await publishEndpoint.Publish(@event);

        logger.LogInformation("Published RideCompletedEvent for RideId: {RideId}", rideId);

        return Ok(new { Message = "Ride completed successfully" });
    }
}

public record RequestRideRequest(
    Guid RiderId,
    string PickupLocation,
    string DropoffLocation
);

public record CancelRideRequest(
    Guid CancelledByUserId,
    string Reason
);

public record CompleteRideRequest(
    Guid RiderId,
    Guid DriverId,
    decimal FareAmount
);


