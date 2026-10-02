using System.Globalization;

namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Evaluates an expression: tokenize, parse, evaluate.
///     <para>Stateless and thread-safe: the tokenizer and parser keep per-call state, so new ones are created on each call.</para>
/// </summary>
public class ExpressionCalculator
{
    private readonly ExpressionEvaluator _evaluator = new();

    /// <exception cref="ExpressionException">The expression is invalid or cannot be calculated.</exception>
    public decimal Evaluate(string? expression)
    {
        var tokens = new ExpressionTokenizer().Tokenize(expression);
        var tree = new ExpressionParser().Parse(tokens);

        return _evaluator.Evaluate(tree);
    }

    /// <summary>
    ///     Formats a result for display: '.' as decimal separator (as in expressions), no trailing zeros
    ///     and no exponent notation.
    /// </summary>
    /// <param name="groupThousands">
    ///     Separate thousands with ',' (1,234,567.5). Off by default: ',' separates function arguments,
    ///     so a grouped result cannot be typed back into an expression.
    /// </param>
    public static string Format(decimal value, bool groupThousands = false)
    {
        // Avoid "-0" from negating zero
        if (value == 0)
        {
            return "0";
        }

        string format = groupThousands ? "#,0.############################" : "0.############################";

        return value.ToString(format, CultureInfo.InvariantCulture);
    }
}
