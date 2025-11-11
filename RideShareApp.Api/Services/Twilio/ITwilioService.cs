namespace RideShareApp.Api.Services.Twilio;

public interface ITwilioService
{
	Task SendMessageAsync(string to, string message, CancellationToken cancellationToken = default);
}


