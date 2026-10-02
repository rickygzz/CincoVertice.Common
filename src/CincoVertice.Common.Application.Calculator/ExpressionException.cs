namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Error found while tokenizing, parsing or evaluating an expression.
/// </summary>
public class ExpressionException : Exception
{
    /// <summary>Zero-based position in the expression where the error was found.</summary>
    public int Position { get; }

    public ExpressionException(string message, int position)
        : base($"{message} at position {position + 1}")
    {
        Position = position;
    }
}
