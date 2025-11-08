using Microsoft.AspNetCore.Mvc;
using RideShareApp.Contracts.Events;
using MassTransit;

namespace RideShareApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriverController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DriverController> _logger;

    public DriverController(IPublishEndpoint publishEndpoint, ILogger<DriverController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost("{rideId}/accept")]
    public async Task<IActionResult> AcceptRide(Guid rideId, [FromBody] AcceptRideRequest request)
    {
        var @event = new RideAcceptedEvent(
            rideId,
            request.RiderId,
            request.DriverId,
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(@event);

        _logger.LogInformation("Published RideAcceptedEvent for RideId: {RideId}, DriverId: {DriverId}", rideId, request.DriverId);

        return Ok(new { Message = "Ride accepted successfully" });
    }
}

public record AcceptRideRequest(
    Guid RiderId,
    Guid DriverId
);


