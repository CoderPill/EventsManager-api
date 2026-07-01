using EventsManager.Core.Common.Time;

namespace EventsManager.Application.Tests.Common;

/// <summary>
/// Fake implementation of <see cref="IDateTimeProvider"/> for deterministic testing.
/// Fixed to Colombia time (UTC-5) on 2026-06-15 05:00:00 (10:00 UTC - 5h).
/// Returns <see cref="DateTimeKind.Unspecified"/> to match <see cref="SystemDateTimeProvider.GetNowColombia"/>.
/// Supports <see cref="SetNow"/>, <see cref="Advance"/>, and <see cref="Reset"/> for test control.
/// </summary>
public sealed class FakeTimeProvider : IDateTimeProvider
{
    internal static readonly DateTime ColombiaNow = new(2026, 6, 15, 5, 0, 0, DateTimeKind.Unspecified);
    private DateTime? _override;

    public DateTime GetNowColombia() => _override ?? ColombiaNow;

    public DateTime GetNowColombia(DateTimeOffset utcDateTime) =>
        utcDateTime.ToOffset(TimeSpan.FromHours(-5)).DateTime;

    public DateTime GetUtcNow() => DateTime.SpecifyKind((_override ?? ColombiaNow).AddHours(5), DateTimeKind.Utc);

    public Task Delay(TimeSpan delay, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public void SetNow(DateTime colombiaTime) => _override = colombiaTime;

    public void Advance(TimeSpan amount) =>
        _override = (_override ?? ColombiaNow).Add(amount);

    public void Reset() => _override = null;
}
