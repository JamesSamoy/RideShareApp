using Microsoft.AspNetCore.Mvc;
using RideShareApp.Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;

namespace RideShareApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<UserController> _logger;

    public UserController(IPublishEndpoint publishEndpoint, ILogger<UserController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }
    
    [HttpGet("test")]
    [Authorize]
    public Task<IActionResult> TestEndpoint()
    {
        return Task.FromResult<IActionResult>(Ok(new { Message = "Test Successful" }));
    }

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

        await _publishEndpoint.Publish(@event);

        _logger.LogInformation("Published UserCreatedEvent for UserId: {UserId}", userId);

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

        await _publishEndpoint.Publish(@event);

        _logger.LogInformation("Published UserUpdatedEvent for UserId: {UserId}", userId);

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


