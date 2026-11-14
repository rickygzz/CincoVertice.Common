using CincoVertice.Common.Application.Calculator.Ast;
using CincoVertice.Common.Application.Calculator.Conversions;
using CincoVertice.Common.Application.Calculator.Models;
using CincoVertice.Common.Application.Calculator.Network;
using CincoVertice.Common.Application.Calculator.TimeZones;
using System.Globalization;

namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Evaluates an expression: tokenize, parse, evaluate.
///     <para>Stateless and thread-safe: the tokenizer and parser keep per-call state, so new ones are created on each call.</para>
/// </summary>
public class ExpressionCalculator
{
    /// <summary>The name of the last answer, which the caller keeps among the names: ans * 2.</summary>
    public const string LastAnswerName = "ans";

    /// <summary>The name of today's date: today + 30 d.</summary>
    public const string TodayName = "today";

    private static readonly IReadOnlyDictionary<string, CalculatorValue> _noNames =
        new Dictionary<string, CalculatorValue>();

    private readonly ExpressionEvaluator _evaluator;
    private readonly TimeProvider _clock;

    public ExpressionCalculator()
        : this(TimeProvider.System)
    {
    }

    /// <param name="clock">Gives today's date, the time now and the local time zone to time conversions.</param>
    public ExpressionCalculator(TimeProvider clock)
        : this(clock, new SystemNetworkAddressReader())
    {
    }

    /// <param name="clock">Gives today's date, the time now and the local time zone to time conversions.</param>
    /// <param name="network">Gives ipconfig() the computer's addresses.</param>
    public ExpressionCalculator(TimeProvider clock, INetworkAddressReader network)
    {
        _clock = clock;
        _evaluator = new ExpressionEvaluator(network);
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
    public string EvaluateToText(string? expression, bool groupThousands = false) =>
        Calculate(expression, groupThousands: groupThousands).Text;

    /// <summary>
    ///     Calculates a line: an expression, or a name given its value (x = 5). It may use names given earlier, and
    ///     the last answer as <c>ans</c>, which the caller keeps: the calculator itself keeps nothing. A date is shown
    ///     with its day of the week, 2026-12-25 (Friday), and a length of time in months and days, 42 days.
    /// </summary>
    /// <param name="expression">The line.</param>
    /// <param name="names">The values of the names given earlier, the last answer among them; any letter case.</param>
    /// <param name="groupThousands">Separate thousands with ','.</param>
    /// <exception cref="ExpressionException">The line is invalid or cannot be calculated.</exception>
    public CalculationResult Calculate(
        string? expression,
        IReadOnlyDictionary<string, CalculatorValue>? names = null,
        bool groupThousands = false)
    {
        ExpressionNode tree = Parse(expression);

        if (tree is TimeConversionNode time)
        {
            return new CalculationResult(TimeZoneConverter.Convert(time, _clock), null);
        }

        DateOnly today = DateOnly.FromDateTime(_clock.GetLocalNow().DateTime);
        CalculatorValue value = _evaluator.Evaluate(tree, names ?? _noNames, today);
        AssignmentNode? assignment = tree as AssignmentNode;
        string text = value.ToText(groupThousands);

        if ((assignment?.Value ?? tree) is ConversionNode conversion)
        {
            text = $"{text} {UnitTable.Find(conversion.To, conversion.ToPosition).Symbol}";
        }

        return new CalculationResult(text, value, assignment?.Name);
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
