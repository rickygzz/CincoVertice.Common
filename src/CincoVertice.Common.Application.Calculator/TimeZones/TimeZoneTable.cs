using System.Globalization;
using System.Text.RegularExpressions;

namespace CincoVertice.Common.Application.Calculator.TimeZones;

/// <summary>
///     The places and time zones available in time conversions (10:30 Mexico to India).
///     <para>
///         A name is a place from <see cref="Places"/> in any letter case, with '_' or a space between words;
///         "local"; UTC or GMT, with an offset or without (UTC-5, GMT+5:30); or any time zone id the system knows,
///         such as America/Lima.
///     </para>
///     <para>
///         Abbreviations like CST or IST are left out on purpose: each one means more than one time zone.
///     </para>
/// </summary>
public static partial class TimeZoneTable
{
    public const string LocalName = "local";

    // UTC offsets in use go from -12 to +14 hours
    private const int MaxOffsetHours = 14;

    public static readonly IReadOnlyList<TimeZonePlace> Places =
    [
        // Mexico has had no daylight saving time since 2022, except along the US border
        new("Mexico City", "America/Mexico_City", ["mexico", "cdmx", "ciudad de mexico"]),
        new("Guadalajara", "America/Mexico_City", []),
        new("Monterrey", "America/Monterrey", []),
        new("Tijuana", "America/Tijuana", []),
        new("Cancun", "America/Cancun", []),

        // United States and Canada
        new("Miami", "America/New_York", []),
        new("New York", "America/New_York", ["nyc"]),
        new("Chicago", "America/Chicago", []),
        new("Denver", "America/Denver", []),
        new("Los Angeles", "America/Los_Angeles", []),
        new("Toronto", "America/Toronto", []),

        // South America
        new("Bogotá", "America/Bogota", ["bogota", "colombia"]),
        new("Cali", "America/Bogota", []),
        new("Medellín", "America/Bogota", ["medellin"]),
        new("Lima", "America/Lima", ["peru"]),
        new("Santiago", "America/Santiago", ["chile"]),
        new("Buenos Aires", "America/Argentina/Buenos_Aires", ["argentina"]),
        new("São Paulo", "America/Sao_Paulo", ["sao paulo", "saopaulo", "brazil", "brasil"]),
        new("Rio de Janeiro", "America/Sao_Paulo", ["rio"]),

        // Europe
        new("London", "Europe/London", []),
        new("Madrid", "Europe/Madrid", ["spain", "espana"]),
        new("Paris", "Europe/Paris", ["france"]),
        new("Berlin", "Europe/Berlin", ["germany"]),

        // Asia and Oceania
        new("India", "Asia/Kolkata", ["delhi", "new delhi", "mumbai", "kolkata", "bangalore", "bengaluru"]),
        new("China", "Asia/Shanghai", ["beijing", "shanghai", "shenzhen"]),
        new("Hong Kong", "Asia/Hong_Kong", []),
        new("Tokyo", "Asia/Tokyo", ["japan"]),
        new("Singapore", "Asia/Singapore", []),
        new("Dubai", "Asia/Dubai", []),
        new("Sydney", "Australia/Sydney", []),
    ];

    private static readonly Dictionary<string, TimeZonePlace> _byName = Places
        .SelectMany(place => place.OtherNames.Append(place.Name), (place, name) => (place, name))
        .ToDictionary(x => x.name, x => x.place, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Finds a place or time zone by any of its names.
    /// </summary>
    /// <param name="name">The name in the expression.</param>
    /// <param name="position">Where the name is in the expression, to report an error.</param>
    /// <param name="local">The time zone "local" means.</param>
    /// <returns>The name to show after a result, and the time zone.</returns>
    /// <exception cref="ExpressionException">There is no such place or time zone.</exception>
    public static (string Name, TimeZoneInfo Zone) Find(string name, int position, TimeZoneInfo local)
    {
        string words = name.Replace('_', ' ');

        if (words.Equals(LocalName, StringComparison.OrdinalIgnoreCase))
        {
            return (LocalName, local);
        }

        if (_byName.TryGetValue(words, out TimeZonePlace? place))
        {
            return (place.Name, FindSystemZone(place.ZoneId));
        }

        Match offset = UtcOffsetPattern().Match(name);

        if (offset.Success)
        {
            return FromUtcOffset(offset, position);
        }

        // America/Lima, or a Windows id such as Pacific Standard Time
        if (TimeZoneInfo.TryFindSystemTimeZoneById(name, out TimeZoneInfo? zone))
        {
            return (name, zone);
        }

        throw new ExpressionException($"Unknown time zone '{name}'", position);
    }

    private static TimeZoneInfo FindSystemZone(string id) =>
        TimeZoneInfo.TryFindSystemTimeZoneById(id, out TimeZoneInfo? zone)
            ? zone
            : throw new InvalidOperationException($"The system doesn't know the time zone {id}.");

    private static (string Name, TimeZoneInfo Zone) FromUtcOffset(Match match, int position)
    {
        if (!match.Groups["hours"].Success)
        {
            return ("UTC", TimeZoneInfo.Utc);
        }

        int hours = int.Parse(match.Groups["hours"].Value, CultureInfo.InvariantCulture);
        int minutes = match.Groups["minutes"].Success
            ? int.Parse(match.Groups["minutes"].Value, CultureInfo.InvariantCulture)
            : 0;

        if (minutes > 59 || hours > MaxOffsetHours || (hours == MaxOffsetHours && minutes > 0))
        {
            throw new ExpressionException(
                $"A UTC offset must be from UTC-{MaxOffsetHours} to UTC+{MaxOffsetHours}",
                position);
        }

        var offset = new TimeSpan(hours, minutes, 0);

        if (match.Groups["sign"].Value == "-")
        {
            offset = -offset;
        }

        string name = TimeZoneConverter.FormatOffset(offset);

        return (name, TimeZoneInfo.CreateCustomTimeZone(name, offset, name, name));
    }

    [GeneratedRegex(
        @"^(?:utc|gmt)(?:(?<sign>[+-])(?<hours>\d{1,2})(?::(?<minutes>\d{2}))?)?$",
        RegexOptions.IgnoreCase)]
    private static partial Regex UtcOffsetPattern();
}
