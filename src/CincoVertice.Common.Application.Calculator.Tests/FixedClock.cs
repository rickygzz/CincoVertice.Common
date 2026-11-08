namespace CincoVertice.Common.Application.Calculator.Tests;

/// <summary>
///     A clock stopped at one moment, in a chosen local time zone.
/// </summary>
public sealed class FixedClock(DateTimeOffset utcNow, string localZoneId) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;

    public override TimeZoneInfo LocalTimeZone { get; } = TimeZoneInfo.FindSystemTimeZoneById(localZoneId);
}
