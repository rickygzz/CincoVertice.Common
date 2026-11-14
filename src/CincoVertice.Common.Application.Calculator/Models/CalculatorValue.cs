using System.Globalization;

namespace CincoVertice.Common.Application.Calculator.Models;

/// <summary>
///     What a calculation gives, and what a name keeps: a number, a date, a length of time between dates, or a
///     text. A length
///     has months apart from days, since a month has no fixed number of days: 2026-01-31 + 1 month is 2026-02-28.
/// </summary>
public sealed record CalculatorValue
{
    private CalculatorValue(
        CalculatorValueKind kind,
        decimal number,
        DateOnly date,
        decimal months,
        decimal days,
        string text = "")
    {
        Text = text;
        Kind = kind;
        Number = number;
        Date = date;
        Months = months;
        Days = days;
    }

    public CalculatorValueKind Kind { get; }

    /// <summary>The number; 0 for a date or a length of time.</summary>
    public decimal Number { get; }

    /// <summary>The date; <see cref="DateOnly.MinValue"/> for a number or a length of time.</summary>
    public DateOnly Date { get; }

    /// <summary>A length of time's months, years being 12; 0 for the others.</summary>
    public decimal Months { get; }

    /// <summary>A length of time's days, weeks being 7; 0 for the others.</summary>
    public decimal Days { get; }

    /// <summary>The text; empty for the others.</summary>
    public string Text { get; }

    public static CalculatorValue FromNumber(decimal number) =>
        new(CalculatorValueKind.Number, number, DateOnly.MinValue, 0, 0);

    public static CalculatorValue FromDate(DateOnly date) => new(CalculatorValueKind.Date, 0, date, 0, 0);

    public static CalculatorValue FromText(string text) =>
        new(CalculatorValueKind.Text, 0, DateOnly.MinValue, 0, 0, text);

    public static CalculatorValue FromDuration(decimal months, decimal days) =>
        new(CalculatorValueKind.Duration, 0, DateOnly.MinValue, months, days);

    /// <summary>
    ///     As the calculator shows it: a number as <see cref="ExpressionCalculator.Format"/> writes it, a date with its
    ///     day of the week (2026-12-25 (Friday)), a length of time in months and days (1 month, 3 days).
    /// </summary>
    /// <param name="groupThousands">Separate a number's thousands with ','.</param>
    public string ToText(bool groupThousands = false) => Kind switch
    {
        CalculatorValueKind.Date =>
            $"{Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)} ({Date.DayOfWeek})",
        CalculatorValueKind.Duration => DurationText(groupThousands),
        CalculatorValueKind.Text => Text,
        _ => ExpressionCalculator.Format(Number, groupThousands),
    };

    private string DurationText(bool groupThousands)
    {
        List<string> parts = [];

        if (Months != 0)
        {
            parts.Add(Counted(Months, "month", groupThousands));
        }

        if (Days != 0 || parts.Count == 0)
        {
            parts.Add(Counted(Days, "day", groupThousands));
        }

        return string.Join(", ", parts);
    }

    private static string Counted(decimal count, string unit, bool groupThousands) =>
        $"{ExpressionCalculator.Format(count, groupThousands)} {unit}{(Math.Abs(count) == 1 ? string.Empty : "s")}";
}
