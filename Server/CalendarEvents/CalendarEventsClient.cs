using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.CalendarEvents;

public class CalendarEventsClient
{
    private static AccessTokenData? _accessToken;
    private static readonly TimeSpan ExpirationThreshold = new(0, 5, 0);
    private static DateTime _accessTokenExpiration;
    private readonly AuthCredentials _authCredentials;
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
    private readonly TimeProvider _timeProvider;

    static CalendarEventsClient()
    {
        _accessToken = null!;
        AccessTokenSemaphore = new SemaphoreSlim(1, 1);
    }

    public CalendarEventsClient(HttpClient httpClient, TimeProvider timeProvider, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        var clientId = config.GetValue<string>("CivicPlus:ClientId");
        var clientSecret = config.GetValue<string>("CivicPlus:ClientSecret");
        _baseUrl = config.GetValue<string>("CivicPlus:BaseUrl");
        _authCredentials = new AuthCredentials(clientId, clientSecret);
    }

    public static SemaphoreSlim AccessTokenSemaphore { get; set; }

    public async Task<AccessTokenData> GetAccessToken()
    {
        var secondsRemaining = (_accessTokenExpiration - _timeProvider.GetUtcNow().DateTime).TotalSeconds;
        if (_accessToken is not null && secondsRemaining <= ExpirationThreshold.TotalSeconds) return _accessToken;

        _accessToken = await FetchToken();
        return _accessToken;
    }

    public async Task<GetCalendarEventsResponse> GetCalendarEvents(int from = 0, int size = 20)
    {
        var accessToken = await GetAccessToken();

        var requestUri = $"{_baseUrl}/{_authCredentials.ClientId}/api/Events?$skip={from}&$top={size}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        await using var responseContentStream = await response.Content.ReadAsStreamAsync();

        var events = await JsonSerializer.DeserializeAsync<GetCalendarEventsResponse>(
            responseContentStream,
            JsonSerializerOptions.Default
        );

        return events ?? throw new Exception("Failed to deserialize calendar events response");
    }

    private async Task<AccessTokenData> FetchToken()
    {
        try
        {
            await AccessTokenSemaphore.WaitAsync();

            var json = JsonSerializer.Serialize(_authCredentials);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/{_authCredentials.ClientId}/api/Auth")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await using var responseContentStream = await response.Content.ReadAsStreamAsync();

            var accessToken = await JsonSerializer.DeserializeAsync<AccessTokenData>(
                responseContentStream,
                JsonSerializerOptions.Default
            );

            return accessToken ?? throw new Exception("Failed to deserialize access token");
        }
        finally
        {
            AccessTokenSemaphore.Release(1);
        }
    }
}

public class GetCalendarEventsResponse(
    long total,
    IEnumerable<CalendarEvent> items)
{
    [JsonPropertyName("total")] public long Total { get; } = total;

    [JsonPropertyName("items")] public IEnumerable<CalendarEvent> Items { get; } = items;
}