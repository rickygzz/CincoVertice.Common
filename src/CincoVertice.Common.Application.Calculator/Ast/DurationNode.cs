using System.Globalization;

namespace CincoVertice.Common.Application.Calculator.Ast;

/// <summary>A length of time, to add to or take from a date: 30 d, 2 wk, 3 months, 1 yr.</summary>
/// <param name="Months">Its months, years being 12.</param>
/// <param name="Days">Its days, weeks being 7.</param>
/// <param name="Position">Where its number is.</param>
public record DurationNode(decimal Months, decimal Days, int Position) : ExpressionNode(Position)
{
    public override string ToString() =>
        $"({Months.ToString(CultureInfo.InvariantCulture)} months {Days.ToString(CultureInfo.InvariantCulture)} days)";
}
