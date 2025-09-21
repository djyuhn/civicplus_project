namespace Server.CalendarEvents;

public class AuthCredentials(
    string clientId,
    string clientSecret)
{
    public string ClientId { get; } = clientId;
    public string ClientSecret { get; } = clientSecret;
}