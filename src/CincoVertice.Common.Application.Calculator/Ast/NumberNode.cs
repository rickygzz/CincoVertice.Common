using System.Globalization;

namespace CincoVertice.Common.Application.Calculator.Ast;

public record NumberNode(decimal Value, int Position) : ExpressionNode(Position)
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
