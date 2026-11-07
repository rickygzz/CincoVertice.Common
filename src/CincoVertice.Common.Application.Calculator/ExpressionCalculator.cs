using CincoVertice.Common.Application.Calculator.Ast;
using CincoVertice.Common.Application.Calculator.Conversions;
using CincoVertice.Common.Application.Calculator.TimeZones;
using System.Globalization;

namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Evaluates an expression: tokenize, parse, evaluate.
///     <para>Stateless and thread-safe: the tokenizer and parser keep per-call state, so new ones are created on each call.</para>
/// </summary>
public class ExpressionCalculator
{
    private readonly ExpressionEvaluator _evaluator = new();
    private readonly TimeProvider _clock;

    public ExpressionCalculator()
        : this(TimeProvider.System)
    {
    }

    /// <param name="clock">Gives today's date, the time now and the local time zone to time conversions.</param>
    public ExpressionCalculator(TimeProvider clock)
    {
        _clock = clock;
    }

    /// <exception cref="ExpressionException">The expression is invalid or cannot be calculated.</exception>
    public decimal Evaluate(string? expression)
    {
        return _evaluator.Evaluate(Parse(expression));
    }

    /// <summary>
    ///     Evaluates an expression and formats the result for display (see <see cref="Format"/>). The result of a unit
    ///     conversion is followed by its unit: 6.213711922 mi. A time conversion is the time and the place:
    ///     22:00 India (UTC+5:30).
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="groupThousands">Separate thousands with ','.</param>
    /// <exception cref="ExpressionException">The expression is invalid or cannot be calculated.</exception>
    public string EvaluateToText(string? expression, bool groupThousands = false)
    {
        ExpressionNode tree = Parse(expression);

        if (tree is TimeConversionNode time)
        {
            return TimeZoneConverter.Convert(time, _clock);
        }

        string result = Format(_evaluator.Evaluate(tree), groupThousands);

        return tree is ConversionNode conversion
            ? $"{result} {UnitTable.Find(conversion.To, conversion.ToPosition).Symbol}"
            : result;
    }

    private static ExpressionNode Parse(string? expression)
    {
        var tokens = new ExpressionTokenizer().Tokenize(expression);

        return new ExpressionParser().Parse(tokens);
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
