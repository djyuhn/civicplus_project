using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
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

    [Fact]
    public async Task GivenGetEventsRequestWithFromAndSize_ShouldCallGetEventsEndpointAndReturnEvents()
    {
        var expectedAuth = new AuthCredentials(_clientId, _clientSecret);
        var expectedAuthJson = JsonSerializer.Serialize(expectedAuth, JsonSerializerOptions.Default);

        var expectedToken = new AccessTokenData("someToken", 123456);
        var expectedAuthResp = JsonSerializer.Serialize(
            expectedToken,
            JsonSerializerOptions.Default
        );

        var expectedEvents = new GetCalendarEventsResponse(
            1,
            new List<CalendarEvent>
            {
                new(
                    "someId",
                    "someTitle",
                    "someDescription",
                    new DateTime(2025, 9, 20, 12, 00, 10, DateTimeKind.Utc),
                    new DateTime(2025, 9, 20, 13, 00, 10, DateTimeKind.Utc)
                )
            });
        var expectedEventsResponse = JsonSerializer.Serialize(
            expectedEvents,
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
                    req.Content.ReadAsStringAsync().Result == expectedAuthJson
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedAuthResp, Encoding.UTF8, "application/json")
            })
            .Verifiable();

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.GetLeftPart(UriPartial.Path) == $"{_baseUrl}/{_clientId}/api/Events" &&
                    req.Headers.Authorization != null &&
                    req.Headers.Authorization.Scheme == "Bearer" &&
                    req.Headers.Authorization.Parameter == expectedToken.AccessToken &&
                    HttpUtility.ParseQueryString(req.RequestUri.Query)["$skip"] == "0" &&
                    HttpUtility.ParseQueryString(req.RequestUri.Query)["$top"] == "20"
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedEventsResponse, Encoding.UTF8, "application/json")
            })
            .Verifiable();

        var actual = await _calendarEventsClient.GetCalendarEvents();

        Assert.Equivalent(expectedEvents, actual);
    }

    [Fact]
    public async Task GivenGetEventRequestWithId_ShouldCallGetEventEndpointAndReturnEvent()
    {
        var expectedAuth = new AuthCredentials(_clientId, _clientSecret);
        var expectedAuthJson = JsonSerializer.Serialize(expectedAuth, JsonSerializerOptions.Default);

        var expectedToken = new AccessTokenData("someToken", 123456);
        var expectedAuthResp = JsonSerializer.Serialize(
            expectedToken,
            JsonSerializerOptions.Default
        );

        var expectedEvent = new CalendarEvent(
            "someId",
            "someTitle",
            "someDescription",
            new DateTime(2025, 9, 20, 12, 00, 10, DateTimeKind.Utc),
            new DateTime(2025, 9, 20, 13, 00, 10, DateTimeKind.Utc)
        );

        var expectedEventResponse = JsonSerializer.Serialize(
            expectedEvent,
            JsonSerializerOptions.Default
        );

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri != null &&
                    req.RequestUri.AbsoluteUri.EndsWith("/api/Auth") &&
                    req.Content != null &&
                    req.Content.ReadAsStringAsync().Result == expectedAuthJson
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedAuthResp, Encoding.UTF8, "application/json")
            })
            .Verifiable();

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.AbsoluteUri == $"{_baseUrl}/{_clientId}/api/Events/{expectedEvent.Id}" &&
                    req.Headers.Authorization != null &&
                    req.Headers.Authorization.Scheme == "Bearer" &&
                    req.Headers.Authorization.Parameter == expectedToken.AccessToken
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedEventResponse, Encoding.UTF8, "application/json")
            })
            .Verifiable();

        var actual = await _calendarEventsClient.GetCalendarEvent(expectedEvent.Id);

        Assert.Equivalent(expectedEvent, actual);
    }
}