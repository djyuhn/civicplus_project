using System.Text.Json.Serialization;

namespace Server.CalendarEvents;

public class AccessTokenData(
    string accessToken,
    int expiresIn)
{
    [JsonPropertyName("access_token")] public string AccessToken { get; } = accessToken;

    /// <value>Property <c>ExpiresIn</c> represents how long the token will be valid for in seconds.</value>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; } = expiresIn;
}