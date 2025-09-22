using Microsoft.AspNetCore.Mvc;
using Server.CalendarEvents;

namespace Server;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddScoped<CalendarEventsClient>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
        });

        var app = builder.Build();
        // app.UseCors("AllowAll");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) app.MapOpenApi();

        app.MapGet("/api/Events", async (CalendarEventsClient calendarEventsClient,
                int skip = 0,
                int top = 20) =>
            {
                var events = await calendarEventsClient.GetCalendarEvents(skip, top);
                return Results.Ok(events);
            })
            .WithName("GetCalendarEvents")
            .Produces<List<GetCalendarEventsResponse>>()
            .WithMetadata(new HttpGetAttribute("/api/Events"));

        app.MapGet("/api/Events/{id}", async (CalendarEventsClient calendarEventsClient,
                string id) =>
            {
                var calendarEvent = await calendarEventsClient.GetCalendarEvent(id);
                return Results.Ok(calendarEvent);
            })
            .WithName("GetCalendarEvent")
            .Produces<CalendarEvent>()
            .WithMetadata(new HttpGetAttribute("/api/Events/{id}"));

        app.MapPost("/api/Events", async (CalendarEventsClient calendarEventsClient,
                CalendarEvent calendarEvent) =>
            {
                var events = await calendarEventsClient.UpsertCalendarEvent(calendarEvent);
                return Results.Ok(events);
            })
            .WithName("UpsertCalendarEvent")
            .Produces<List<GetCalendarEventsResponse>>()
            .WithMetadata(new HttpPostAttribute("/api/Events"));

        app.Run();

        app.Run();
    }
}