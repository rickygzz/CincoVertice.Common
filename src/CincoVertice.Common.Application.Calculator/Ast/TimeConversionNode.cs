namespace CincoVertice.Common.Application.Calculator.Ast;

/// <summary>
///     A time converted from one time zone to another, e.g. 10:30 Mexico to India. Only valid as a whole expression.
/// </summary>
/// <param name="Time">The time of day, or null for now.</param>
/// <param name="TwelveHour">The time was written with am or pm, so the result is shown that way too.</param>
/// <param name="TimePosition">Position of the time, or of the word 'now'.</param>
/// <param name="From">The place or time zone the time is in, or null for the local time zone.</param>
/// <param name="FromPosition">Position of the name of the place it is in.</param>
/// <param name="To">The place or time zone to convert it to.</param>
/// <param name="ToPosition">Position of the name of the place to convert it to.</param>
/// <param name="Position">Position of the word 'to'.</param>
public record TimeConversionNode(
    TimeOnly? Time,
    bool TwelveHour,
    int TimePosition,
    string? From,
    int FromPosition,
    string To,
    int ToPosition,
    int Position)
    : ExpressionNode(Position)
{
    public override string ToString() =>
        $"(time {Time?.ToString("HH:mm") ?? "now"}{(TwelveHour ? " 12h" : "")} {From ?? "local"} {To})";
}
