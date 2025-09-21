using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Moq.Protected;
using Server.CalendarEvents;

namespace Server.Tests.CalendarEvents;

public class CalendarEventsClientTests : IDisposable
{
    private readonly string _baseUrl;
    private readonly CalendarEventsClient _calendarEventsClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly IConfigurationRoot _config;
    private readonly HttpClient _httpClient;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly TimeProvider _timeProvider;

    public CalendarEventsClientTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object);

        _clientId = "SomeClientId";
        _clientSecret = "SomeClientSecret";
        _baseUrl = "https://api.example.com";

        var inMemorySettings = new Dictionary<string, string?>
        {
            { "CivicPlus:ClientId", _clientId },
            { "CivicPlus:ClientSecret", _clientSecret },
            { "CivicPlus:BaseUrl", _baseUrl }
        };
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _timeProvider = new FakeTimeProvider(new DateTime(2026, 9, 20));

        _calendarEventsClient = new CalendarEventsClient(_httpClient, _timeProvider, _config);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _httpClient?.Dispose();
        _mockHandler?.Object?.Dispose();
    }

    [Fact]
    public async Task GivenRequest_ShouldCallAuthEndpointAndGetBearerToken()
    {
        var expectedBody = new AuthCredentials(_clientId, _clientSecret);
        var expectedBodyJson = JsonSerializer.Serialize(expectedBody, JsonSerializerOptions.Default);

        var expectedToken = new AccessTokenData("someToken", 123456);
        var jsonResponse = JsonSerializer.Serialize(
            expectedToken,
            JsonSerializerOptions.Default
        );

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri != null &&
                    req.RequestUri.AbsoluteUri == $"{_baseUrl}/{_clientId}/api/Auth" &&
                    req.Content != null &&
                    req.Content.ReadAsStringAsync().Result == expectedBodyJson
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });

        var actual = await _calendarEventsClient.GetAccessToken();

        Assert.Equivalent(expectedToken, actual);
    }
}