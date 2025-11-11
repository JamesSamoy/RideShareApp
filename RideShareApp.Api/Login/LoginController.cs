using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RideShareApp.Api.Authentication;
using RideShareApp.Api.Services.Twilio;

namespace RideShareApp.Api.Login;

[ApiController]
[Route("api/[controller]")]
public class LoginController(
    ILogger<LoginController> _logger, 
    IDistributedCache _cache, 
    ITwilioService _twilioService, 
    TokenService _tokenService) : ControllerBase
{
    // Retry/lockout settings
    private const int MaxRequestAttempts = 5;
    private static readonly TimeSpan RequestWindow = TimeSpan.FromMinutes(15);
    private const int MaxVerifyAttempts = 6;
    private static readonly TimeSpan VerifyWindow = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{7,14}$", RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    [HttpPost("PhoneNumber")]
    public async Task<IActionResult> RequestCodeViaPhoneNumber([FromBody] RequestCodeViaPhoneNumberRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Number) || !PhoneRegex.IsMatch(request.Number))
        {
            return BadRequest("Invalid phone number format.");
        }

        if (await IsLockedOutAsync(request.Number))
        {
            return StatusCode(429, "Too many attempts. Please try again later.");
        }

        var reqCount = await IncrementCounterAsync(RequestCountKey(request.Number), RequestWindow);
        if (reqCount > MaxRequestAttempts)
        {
            await LockoutAsync(request.Number);
            return StatusCode(429, "Too many code requests. Please try again later.");
        }

        var code = GenerateCode();
        var payload = JsonSerializer.Serialize(new { Code = code, Contact = request.Number, Type = "phone" });
        await _cache.SetStringAsync(CacheKey(request.Number), payload, DefaultCacheExpiry());

        var message = $"Your RideShare verification code is: {code}";
        await _twilioService.SendMessageAsync(request.Number, message);

        return Ok();
    }
    
    [HttpPost("Email")]
    public async Task<IActionResult> RequestCodeViaEmail([FromBody] RequestCodeViaEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !EmailRegex.IsMatch(request.Email))
        {
            return BadRequest("Invalid email format.");
        }

        if (await IsLockedOutAsync(request.Email))
        {
            return StatusCode(429, "Too many attempts. Please try again later.");
        }

        var reqCount = await IncrementCounterAsync(RequestCountKey(request.Email), RequestWindow);
        if (reqCount > MaxRequestAttempts)
        {
            await LockoutAsync(request.Email);
            return StatusCode(429, "Too many code requests. Please try again later.");
        }

        var code = GenerateCode();
        var payload = JsonSerializer.Serialize(new { Code = code, Contact = request.Email, Type = "email" });
        await _cache.SetStringAsync(CacheKey(request.Email), payload, DefaultCacheExpiry());

        var message = $"Your RideShare verification code is: {code}";
        await _twilioService.SendMessageAsync(request.Email, message);

        return Ok();
    }

    [HttpPost("VerifyCode")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest? request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.ContactInfo))
        {
            return BadRequest("Code and contactInfo are required.");
        }

        if (await IsLockedOutAsync(request.ContactInfo))
        {
            return StatusCode(429, "Too many attempts. Please try again later.");
        }

        var attempts = await IncrementCounterAsync(VerifyCountKey(request.ContactInfo), VerifyWindow);
        if (attempts > MaxVerifyAttempts)
        {
            await LockoutAsync(request.ContactInfo);
            return StatusCode(429, "Too many verification attempts. Please try again later.");
        }

        var cached = await _cache.GetStringAsync(CacheKey(request.ContactInfo));
        if (string.IsNullOrEmpty(cached))
        {
            return NotFound("Verification code not found or expired.");
        }

        var data = JsonSerializer.Deserialize<VerificationPayload>(cached);
        if (data is null || !string.Equals(data.Code, request.Code, StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized("Invalid verification code.");
        }

        // Remove the code once verified
        await _cache.RemoveAsync(CacheKey(request.ContactInfo));
        
        // Reset verification attempts on success
        await _cache.RemoveAsync(VerifyCountKey(request.ContactInfo));

        // In a real app, look up or create the user and return their profile and a token
        var userId = Guid.NewGuid().ToString();
        var token = _tokenService.GenerateToken(userId);

        return Ok(new
        {
            User = new
            {
                Id = userId,
                Contact = request.ContactInfo
            },
            Token = token
        });
    }
    
    private static DistributedCacheEntryOptions DefaultCacheExpiry() =>
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    private static DistributedCacheEntryOptions MakeExpiry(TimeSpan window) =>
        new() { AbsoluteExpirationRelativeToNow = window };

    private async Task<bool> IsLockedOutAsync(string contact)
    {
        var locked = await _cache.GetStringAsync(LockKey(contact));
        return !string.IsNullOrEmpty(locked);
    }

    private async Task<int> GetCounterAsync(string key)
    {
        var value = await _cache.GetStringAsync(key);
        return int.TryParse(value, out var count) ? count : 0;
    }

    private async Task<int> IncrementCounterAsync(string key, TimeSpan window)
    {
        // IDistributedCache has no atomic increment; this is acceptable for demo purposes.
        var current = await GetCounterAsync(key);
        current++;
        await _cache.SetStringAsync(key, current.ToString(), MakeExpiry(window));
        return current;
    }

    private async Task LockoutAsync(string contact)
    {
        await _cache.SetStringAsync(LockKey(contact), "1", MakeExpiry(LockoutDuration));
    }
    
    private static string GenerateCode(int length = 6)
    {
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = Random.Shared;
        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            chars[i] = alphabet[random.Next(alphabet.Length)];
        }
        return new string(chars);
    }

    private static string CacheKey(string contact) => $"login:code:{contact}".ToLowerInvariant();
    private static string RequestCountKey(string contact) => $"login:req:count:{contact}".ToLowerInvariant();
    private static string VerifyCountKey(string contact) => $"login:verify:count:{contact}".ToLowerInvariant();
    private static string LockKey(string contact) => $"login:lock:{contact}".ToLowerInvariant();

    public sealed record VerifyCodeRequest(string Code, string ContactInfo);
    private sealed record VerificationPayload(string Code, string Contact, string Type);

    public sealed record RequestCodeViaPhoneNumberRequest(string Number);

    public sealed record RequestCodeViaEmailRequest(string Email);
}