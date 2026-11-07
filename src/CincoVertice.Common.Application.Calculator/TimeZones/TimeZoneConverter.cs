using CincoVertice.Common.Application.Calculator.Ast;
using System.Globalization;

namespace CincoVertice.Common.Application.Calculator.TimeZones;

/// <summary>
///     Converts a time between time zones and writes the result: 10:30 Mexico to India is 22:00 India (UTC+5:30).
///     <para>
///         The time is taken as today's in the place it is in, because daylight saving time can change the
///         difference: 10:00 Mexico City is 11:00 in Miami in winter and 12:00 in summer.
///     </para>
/// </summary>
public static class TimeZoneConverter
{
    /// <param name="node">The conversion.</param>
    /// <param name="clock">Gives today's date, the time now and the local time zone.</param>
    /// <exception cref="ExpressionException">
    ///     A place is unknown, or the time doesn't exist there today because the clocks move forward.
    /// </exception>
    public static string Convert(TimeConversionNode node, TimeProvider clock)
    {
        TimeZoneInfo local = clock.LocalTimeZone;
        (string fromName, TimeZoneInfo fromZone) = node.From is null
            ? (TimeZoneTable.LocalName, local)
            : TimeZoneTable.Find(node.From, node.FromPosition, local);
        (string toName, TimeZoneInfo toZone) = TimeZoneTable.Find(node.To, node.ToPosition, local);

        DateTime fromNow = TimeZoneInfo.ConvertTime(clock.GetUtcNow(), fromZone).DateTime;
        DateTime fromTime = node.Time is { } time ? fromNow.Date + time.ToTimeSpan() : fromNow;

        if (fromZone.IsInvalidTime(fromTime))
        {
            throw new ExpressionException(
                $"{FormatTime(fromTime, node.TwelveHour)} doesn't exist in {fromName} today: the clocks move forward",
                node.TimePosition);
        }

        // A time that happens twice, when the clocks move back, is taken as the first one, in standard time
        var from = new DateTimeOffset(fromTime, fromZone.GetUtcOffset(fromTime));
        DateTimeOffset to = TimeZoneInfo.ConvertTime(from, toZone);
        int days = (to.Date - from.Date).Days;

        // UTC-5 (UTC-5) would say it twice
        string offset = FormatOffset(to.Offset);
        string place = toName == offset ? toName : $"{toName} ({offset})";

        return $"{FormatTime(to.DateTime, node.TwelveHour)} {place}{FormatDays(days)}";
    }

    /// <summary>
    ///     Writes a UTC offset the short way: UTC, UTC-5, UTC+5:30.
    /// </summary>
    public static string FormatOffset(TimeSpan offset)
    {
        if (offset == TimeSpan.Zero)
        {
            return "UTC";
        }

        string sign = offset < TimeSpan.Zero ? "-" : "+";
        TimeSpan size = offset.Duration();

        return size.Minutes == 0
            ? $"UTC{sign}{size.Hours}"
            : $"UTC{sign}{size.Hours}:{size.Minutes:00}";
    }

    private static string FormatTime(DateTime time, bool twelveHour)
    {
        if (!twelveHour)
        {
            return time.ToString("H:mm", CultureInfo.InvariantCulture);
        }

        string half = time.Hour < 12 ? "am" : "pm";

        return $"{time.ToString("h:mm", CultureInfo.InvariantCulture)} {half}";
    }

    private static string FormatDays(int days) => days switch
    {
        0 => "",
        1 => ", next day",
        -1 => ", previous day",
        _ => $", {days:+0;-0} days",
    };
}
