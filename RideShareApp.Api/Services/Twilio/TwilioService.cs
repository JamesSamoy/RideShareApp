using Microsoft.Extensions.Logging;

namespace RideShareApp.Api.Services.Twilio;

public class TwilioService(ILogger<TwilioService> logger) : ITwilioService
{
	public Task SendMessageAsync(string to, string message, CancellationToken cancellationToken = default)
	{
		// Placeholder implementation. Integrate real Twilio client here later.
		logger.LogInformation("Sending message to {To}: {Message}", to, message);
		return Task.CompletedTask;
	}
}


