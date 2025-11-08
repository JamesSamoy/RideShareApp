using Microsoft.AspNetCore.Mvc;
using RideShareApp.Contracts.Events;
using MassTransit;

namespace RideShareApp.RatingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RatingController> _logger;

    public RatingController(IPublishEndpoint publishEndpoint, ILogger<RatingController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitRating([FromBody] SubmitRatingRequest request)
    {
        var ratingId = Guid.NewGuid();
        var @event = new RatingSubmittedEvent(
            ratingId,
            request.RideId,
            request.RatedByUserId,
            request.RatedUserId,
            request.Rating,
            request.Comment,
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(@event);

        _logger.LogInformation("Published RatingSubmittedEvent for RatingId: {RatingId}, RideId: {RideId}", ratingId, request.RideId);

        return Ok(new { RatingId = ratingId, Message = "Rating submitted successfully" });
    }

    [HttpGet("user/{userId}")]
    public Task<IActionResult> GetUserRatings(Guid userId)
    {
        // In a real application, this would query the database for ratings
        _logger.LogInformation("Getting ratings for user {UserId}", userId);
        
        return Task.FromResult<IActionResult>(Ok(new { UserId = userId, AverageRating = 4.5, TotalRatings = 10 }));
    }
}

public record SubmitRatingRequest(
    Guid RideId,
    Guid RatedByUserId,
    Guid RatedUserId,
    int Rating,
    string? Comment
);

