using CincoVertice.Common.Application.Calculator.Ast;
using CincoVertice.Common.Application.Calculator.Conversions;
using CincoVertice.Common.Application.Calculator.Functions;
using CincoVertice.Common.Application.Calculator.Models;

namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Calculates the value of a syntax tree built by <see cref="ExpressionParser"/>: a number, a date, or a length of
///     time. A date moves by a length of time or a number of days; two dates are a length of time apart.
/// </summary>
public class ExpressionEvaluator
{
    // Enough for any practical conversion, instead of the 28 decimals of a result like 1 km to mi
    private const int ConversionSignificantDigits = 10;

    private static readonly IReadOnlyDictionary<string, CalculatorValue> _noNames =
        new Dictionary<string, CalculatorValue>();

    /// <summary>The number an expression gives, with no names but the constants; today is the computer's.</summary>
    /// <exception cref="ExpressionException">
    ///     Division by zero, overflow, unknown name or function, wrong argument count, invalid function input, a unit
    ///     that is unknown or can't be converted to the other, or a value that isn't a number.
    /// </exception>
    public decimal Evaluate(ExpressionNode node) =>
        NumberOf(Evaluate(node, _noNames, DateOnly.FromDateTime(DateTime.Today)), node.Position);

    /// <param name="node">What to calculate.</param>
    /// <param name="names">The values of the names it can use, such as the last answer; any letter case.</param>
    /// <param name="today">What today is.</param>
    /// <exception cref="ExpressionException">The calculation can't be done.</exception>
    public CalculatorValue Evaluate(
        ExpressionNode node,
        IReadOnlyDictionary<string, CalculatorValue> names,
        DateOnly today) => node switch
    {
        NumberNode number => CalculatorValue.FromNumber(number.Value),
        DateNode date => CalculatorValue.FromDate(date.Date),
        DurationNode duration => CalculatorValue.FromDuration(duration.Months, duration.Days),
        IdentifierNode identifier => EvaluateIdentifier(identifier, names, today),
        UnaryNode unary => EvaluateUnary(unary, names, today),
        BinaryNode binary => EvaluateBinary(binary, names, today),
        FunctionCallNode functionCall => EvaluateFunctionCall(functionCall, names, today),
        ConversionNode conversion => EvaluateConversion(conversion, names, today),
        AssignmentNode assignment => Evaluate(assignment.Value, names, today),
        // Written by ExpressionCalculator.Calculate instead
        TimeConversionNode time => throw new ExpressionException("A time has no number value", time.Position),
        // A text is only valid as a function argument (see EvaluateArgument)
        StringNode text => throw new ExpressionException("Text can only be used as a function argument", text.Position),
        _ => throw new ArgumentException($"Unknown node type '{node.GetType().Name}'", nameof(node))
    };

    private static decimal NumberOf(CalculatorValue value, int position) => value.Kind switch
    {
        CalculatorValueKind.Number => value.Number,
        CalculatorValueKind.Date => throw new ExpressionException("Expected a number, not a date", position),
        _ => throw new ExpressionException("Expected a number, not a length of time", position),
    };

    private static CalculatorValue EvaluateIdentifier(
        IdentifierNode node,
        IReadOnlyDictionary<string, CalculatorValue> names,
        DateOnly today)
    {
        if (BuiltIns.Constants.TryGetValue(node.Name, out decimal value))
        {
            return CalculatorValue.FromNumber(value);
        }

        if (node.Name.Equals(ExpressionCalculator.TodayName, StringComparison.OrdinalIgnoreCase))
        {
            return CalculatorValue.FromDate(today);
        }

        if (names.FirstOrDefault(name => name.Key.Equals(node.Name, StringComparison.OrdinalIgnoreCase))
            is { Value: { } named })
        {
            return named;
        }

        throw new ExpressionException(
            node.Name.Equals(ExpressionCalculator.LastAnswerName, StringComparison.OrdinalIgnoreCase)
                ? "There's no last answer yet"
                : $"Unknown name '{node.Name}'",
            node.Position);
    }

    private CalculatorValue EvaluateUnary(
        UnaryNode node,
        IReadOnlyDictionary<string, CalculatorValue> names,
        DateOnly today)
    {
        CalculatorValue operand = Evaluate(node.Operand, names, today);

        if (node.Operator == UnaryOperator.Plus)
        {
            return operand;
        }

        // decimal is symmetric, so negation cannot overflow
        return operand.Kind switch
        {
            CalculatorValueKind.Number => CalculatorValue.FromNumber(-operand.Number),
            CalculatorValueKind.Duration => CalculatorValue.FromDuration(-operand.Months, -operand.Days),
            _ => throw new ExpressionException("A date can't be negative", node.Position),
        };
    }

    private CalculatorValue EvaluateBinary(
        BinaryNode node,
        IReadOnlyDictionary<string, CalculatorValue> names,
        DateOnly today)
    {
        CalculatorValue left = Evaluate(node.Left, names, today);
        CalculatorValue right = Evaluate(node.Right, names, today);

        if (left.Kind == CalculatorValueKind.Number && right.Kind == CalculatorValueKind.Number)
        {
            return CalculatorValue.FromNumber(Calculate(node.Position, () => node.Operator switch
            {
                BinaryOperator.Add => left.Number + right.Number,
                BinaryOperator.Subtract => left.Number - right.Number,
                BinaryOperator.Multiply => left.Number * right.Number,
                BinaryOperator.Divide => left.Number / right.Number,
                // Result has the sign of the dividend: -7 % 3 = -1
                BinaryOperator.Modulo => left.Number % right.Number,
                BinaryOperator.Power => DecimalMath.Pow(left.Number, right.Number),
                _ => throw new ArgumentOutOfRangeException(nameof(node), node.Operator, null)
            }));
        }

        return EvaluateTime(node, left, right);
    }

    /// <summary>
    ///     Dates and lengths of time: a date and a length of time, or a number of days, make a date; two dates a length
    ///     of time; two lengths of time another; and a length of time can be multiplied or divided by a number.
    /// </summary>
    private static CalculatorValue EvaluateTime(BinaryNode node, CalculatorValue left, CalculatorValue right)
    {
        CalculatorValueKind leftKind = left.Kind;
        CalculatorValueKind rightKind = right.Kind;

        return (node.Operator, leftKind, rightKind) switch
        {
            (BinaryOperator.Add or BinaryOperator.Subtract, CalculatorValueKind.Date, not CalculatorValueKind.Date) =>
                Moved(left.Date, right, node.Operator == BinaryOperator.Subtract ? -1 : 1, node.Position),
            (BinaryOperator.Add, not CalculatorValueKind.Date, CalculatorValueKind.Date) =>
                Moved(right.Date, left, 1, node.Position),
            (BinaryOperator.Subtract, CalculatorValueKind.Date, CalculatorValueKind.Date) =>
                CalculatorValue.FromDuration(0, left.Date.DayNumber - right.Date.DayNumber),
            (BinaryOperator.Add or BinaryOperator.Subtract, CalculatorValueKind.Duration,
                CalculatorValueKind.Duration) =>
                Combined(left, right, node.Operator == BinaryOperator.Subtract ? -1 : 1),
            (BinaryOperator.Multiply, CalculatorValueKind.Duration, CalculatorValueKind.Number) =>
                Scaled(left, right.Number, node.Position),
            (BinaryOperator.Multiply, CalculatorValueKind.Number, CalculatorValueKind.Duration) =>
                Scaled(right, left.Number, node.Position),
            (BinaryOperator.Divide, CalculatorValueKind.Duration, CalculatorValueKind.Number) =>
                Scaled(left, Calculate(node.Position, () => 1 / right.Number), node.Position),
            _ => throw new ExpressionException(
                $"Can't {Verb(node.Operator)} {Article(leftKind)} and {Article(rightKind)}",
                node.Position),
        };
    }

    /// <summary>The date moved by a length of time, or a number of days: its months first, then its days.</summary>
    private static CalculatorValue Moved(DateOnly date, CalculatorValue by, int sign, int position)
    {
        decimal months = by.Kind == CalculatorValueKind.Duration ? by.Months : 0;
        decimal days = by.Kind == CalculatorValueKind.Duration ? by.Days : by.Number;

        if (months != decimal.Truncate(months) || days != decimal.Truncate(days))
        {
            throw new ExpressionException("A date moves by whole days and whole months", position);
        }

        try
        {
            return CalculatorValue.FromDate(date.AddMonths((int)months * sign).AddDays((int)days * sign));
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException or OverflowException)
        {
            throw new ExpressionException("The date is out of range", position);
        }
    }

    private static CalculatorValue Combined(CalculatorValue left, CalculatorValue right, int sign) =>
        CalculatorValue.FromDuration(left.Months + (sign * right.Months), left.Days + (sign * right.Days));

    private static CalculatorValue Scaled(CalculatorValue duration, decimal by, int position) =>
        CalculatorValue.FromDuration(
            Calculate(position, () => duration.Months * by),
            Calculate(position, () => duration.Days * by));

    private static string Verb(BinaryOperator op) => op switch
    {
        BinaryOperator.Add => "add",
        BinaryOperator.Subtract => "subtract",
        BinaryOperator.Multiply => "multiply",
        BinaryOperator.Divide => "divide",
        BinaryOperator.Modulo => "take the remainder of",
        _ => "raise",
    };

    private static string Article(CalculatorValueKind kind) => kind switch
    {
        CalculatorValueKind.Date => "a date",
        CalculatorValueKind.Duration => "a length of time",
        _ => "a number",
    };

    private CalculatorValue EvaluateConversion(
        ConversionNode node,
        IReadOnlyDictionary<string, CalculatorValue> names,
        DateOnly today)
    {
        UnitDefinition from = UnitTable.Find(node.From, node.FromPosition);
        UnitDefinition to = UnitTable.Find(node.To, node.ToPosition);

        if (from.Group != to.Group)
        {
            throw new ExpressionException($"Can't convert {from.Symbol} to {to.Symbol}", node.Position);
        }

        decimal value = NumberOf(Evaluate(node.Value, names, today), node.Value.Position);

        return CalculatorValue.FromNumber(Calculate(
            node.Position,
            () => DecimalMath.RoundToSignificantDigits(to.FromBase(from.ToBase(value)), ConversionSignificantDigits)));
    }

    private CalculatorValue EvaluateFunctionCall(
        FunctionCallNode node,
        IReadOnlyDictionary<string, CalculatorValue> names,
        DateOnly today)
    {
        if (!BuiltIns.Functions.TryGetValue(node.Name, out FunctionDefinition? function))
        {
            throw new ExpressionException($"Unknown function '{node.Name}'", node.Position);
        }

        if (!function.AcceptsArgumentCount(node.Arguments.Count))
        {
            throw new ExpressionException(
                $"Function '{function.Name}' expects {function.DescribeArgumentCount()}",
                node.Position);
        }

        List<FunctionArgument> arguments =
            [.. node.Arguments.Select(argument => EvaluateArgument(argument, names, today))];

        return CalculatorValue.FromNumber(Calculate(node.Position, () => function.Invoke(arguments)));
    }

    private FunctionArgument EvaluateArgument(
        ExpressionNode node,
        IReadOnlyDictionary<string, CalculatorValue> names,
        DateOnly today) => node is StringNode text
        ? new FunctionArgument(text.Value, text.Position)
        : new FunctionArgument(NumberOf(Evaluate(node, names, today), node.Position), node.Position);

    /// <summary>
    ///     Runs an operation and reports arithmetic errors at the given position.
    /// </summary>
    private static decimal Calculate(int position, Func<decimal> operation)
    {
        try
        {
            return operation();
        }
        catch (DivideByZeroException)
        {
            throw new ExpressionException("Division by zero", position);
        }
        catch (OverflowException)
        {
            throw new ExpressionException("Number is too large", position);
        }
        catch (ArithmeticException ex)
        {
            throw new ExpressionException(ex.Message, position);
        }
    }
}
