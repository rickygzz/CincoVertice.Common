namespace CincoVertice.Common.Application.Calculator.TimeZones;

/// <summary>
///     A place whose time can be converted (10:30 Mexico to India).
/// </summary>
/// <param name="Name">Name shown after a result.</param>
/// <param name="ZoneId">IANA id of its time zone, which knows its daylight saving time rules.</param>
/// <param name="OtherNames">Other names used in expressions, e.g. "cdmx".</param>
public sealed record TimeZonePlace(string Name, string ZoneId, string[] OtherNames);
