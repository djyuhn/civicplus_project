using System.Text.Json.Serialization;

namespace Server.CalendarEvents;

public class CalendarEvent(
    string id,
    string title,
    string description,
    DateTime startDate,
    DateTime endDate
)
{
    [JsonPropertyName("id")] public string Id { get; } = id;
    [JsonPropertyName("title")] public string Title { get; } = title;
    [JsonPropertyName("description")] public string Description { get; } = description;
    [JsonPropertyName("startDate")] public DateTime StartDate { get; } = startDate;
    [JsonPropertyName("endDate")] public DateTime EndDate { get; } = endDate;
}