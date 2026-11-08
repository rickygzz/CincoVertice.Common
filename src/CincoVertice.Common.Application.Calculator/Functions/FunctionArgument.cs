namespace CincoVertice.Common.Application.Calculator.Functions;

/// <summary>
///     Evaluated function argument: a number or a text.
/// </summary>
/// <param name="Value">A decimal or a string.</param>
/// <param name="Position">Position of the argument in the expression, used to report a wrong type.</param>
public record FunctionArgument(object Value, int Position)
{
    /// <exception cref="ExpressionException">The argument is not a number.</exception>
    public decimal Number => Value as decimal? ?? throw new ExpressionException("Expected a number", Position);

    /// <exception cref="ExpressionException">The argument is not a text.</exception>
    public string Text => Value as string ?? throw new ExpressionException("Expected text", Position);
}
