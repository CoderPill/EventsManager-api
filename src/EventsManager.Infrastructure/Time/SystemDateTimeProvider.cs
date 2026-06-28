using EventsManager.Core.Common.Time;

namespace EventsManager.Infrastructure.Time;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    private static readonly TimeZoneInfo ColombiaZone = 
        TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); // Windows
        // TimeZoneInfo.FindSystemTimeZoneById("America/Bogota"); // Linux/Docker

    public DateTime GetNowColombia()
        => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, ColombiaZone).DateTime;

    public DateTime GetNowColombia(DateTimeOffset utcDateTime)
        => TimeZoneInfo.ConvertTime(utcDateTime, ColombiaZone).DateTime;

    public DateTime GetUtcNow()
        => DateTime.UtcNow;

    public Task Delay(TimeSpan delay, CancellationToken cancellationToken = default)
        => Task.Delay(delay, cancellationToken);
}