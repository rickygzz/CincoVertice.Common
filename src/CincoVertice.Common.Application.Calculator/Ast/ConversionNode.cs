namespace CincoVertice.Common.Application.Calculator.Ast;

/// <summary>
///     A unit conversion, e.g. 10 km to mi. Only valid as a whole expression.
/// </summary>
/// <param name="Value">The value to convert.</param>
/// <param name="From">The name of the unit it is in.</param>
/// <param name="FromPosition">Position of the name of the unit it is in.</param>
/// <param name="To">The name of the unit to convert it to.</param>
/// <param name="ToPosition">Position of the name of the unit to convert it to.</param>
/// <param name="Position">Position of the word 'to'.</param>
public record ConversionNode(
    ExpressionNode Value,
    string From,
    int FromPosition,
    string To,
    int ToPosition,
    int Position)
    : ExpressionNode(Position)
{
    public override string ToString() => $"(to {Value} {From} {To})";
}
