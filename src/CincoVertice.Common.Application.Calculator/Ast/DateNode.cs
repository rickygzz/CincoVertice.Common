using System.Globalization;

namespace CincoVertice.Common.Application.Calculator.Ast;

/// <summary>A date written as year-month-day: 2026-12-25.</summary>
public record DateNode(DateOnly Date, int Position) : ExpressionNode(Position)
{
    public override string ToString() => Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}
