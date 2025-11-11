using Microsoft.AspNetCore.Mvc;
using RideShareApp.Api.Authentication;

namespace RideShareApp.Api.Controllers;

[ApiController]
[Route("api/token")]
public class AuthenticationController(
    TokenService tokenService,
    ILogger<AuthenticationController> logger)
    : ControllerBase
{

    [HttpGet("{userId}")]
    public OkObjectResult GetToken(string userId)
    {
        var token = tokenService.GenerateToken(userId);
        
        logger.LogInformation("Generated Token [{token}] for user: {userId}", token, userId);

        return Ok(new { Token = token});
    }
}