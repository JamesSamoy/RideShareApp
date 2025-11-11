using Microsoft.AspNetCore.Mvc;
using RideShareApp.Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;

namespace RideShareApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IPublishEndpoint publishEndpoint, ILogger<UserController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var userId = Guid.NewGuid();
        var @event = new UserCreatedEvent(
            userId,
            request.Name,
            request.Email,
            request.PhoneNumber,
            DateTime.UtcNow
        );

        await publishEndpoint.Publish(@event);

        logger.LogInformation("Published UserCreatedEvent for UserId: {UserId}", userId);

        return Ok(new { UserId = userId, Message = "User created successfully" });
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
    {
        var @event = new UserUpdatedEvent(
            userId,
            request.Name,
            request.Email,
            request.PhoneNumber,
            DateTime.UtcNow
        );

        await publishEndpoint.Publish(@event);

        logger.LogInformation("Published UserUpdatedEvent for UserId: {UserId}", userId);

        return Ok(new { Message = "User updated successfully" });
    }
}

public record CreateUserRequest(
    string Name,
    string Email,
    string PhoneNumber
);

public record UpdateUserRequest(
    string? Name,
    string? Email,
    string? PhoneNumber
);


