namespace CincoVertice.Common.Application.Calculator.Functions;

/// <summary>
///     Constants and functions available in expressions. Names are case-insensitive.
/// </summary>
public static class BuiltIns
{
    public static readonly IReadOnlyDictionary<string, decimal> Constants =
        new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            ["pi"] = 3.1415926535897932384626433833m,
            ["e"] = 2.7182818284590452353602874714m,
        };

    public static readonly IReadOnlyDictionary<string, FunctionDefinition> Functions =
        new FunctionDefinition[]
        {
            new("abs", 1, 1, a => Math.Abs(a[0].Number)),
            new("sqrt", 1, 1, a => DecimalMath.Sqrt(a[0].Number)),
            // round(x) or round(x, decimals). Midpoints round away from zero: round(2.5) = 3
            new("round", 1, 2, a => Math.Round(a[0].Number, a.Count > 1 ? ToDecimals(a[1].Number) : 0, MidpointRounding.AwayFromZero)),
            new("floor", 1, 1, a => Math.Floor(a[0].Number)),
            new("ceil", 1, 1, a => Math.Ceiling(a[0].Number)),
            new("min", 1, int.MaxValue, a => a.Min(x => x.Number)),
            new("max", 1, int.MaxValue, a => a.Max(x => x.Number)),
            // Trigonometric functions use radians
            new("sin", 1, 1, a => Trigonometric(Math.Sin, a[0].Number)),
            new("cos", 1, 1, a => Trigonometric(Math.Cos, a[0].Number)),
            new("tan", 1, 1, a => Trigonometric(Math.Tan, a[0].Number)),
            new("ln", 1, 1, a => DecimalMath.Ln(a[0].Number)),
            new("log", 1, 1, a => DecimalMath.Log10(a[0].Number)),
            new("exp", 1, 1, a => DecimalMath.FromDouble(Math.Exp((double)a[0].Number))),
            // Text
            new("len", 1, 1, a => a[0].Text.Length),
            // find(part, text) or find(part, text, start), as Excel FIND
            new("find", 2, 3, a => Find(a[0].Text, a[1].Text, a.Count > 2 ? a[2].Number : 1)),
        }
        .ToDictionary(f => f.Name, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     double trigonometry is only accurate to about 1e-16, so sin(pi) gives 1.2E-16 instead of 0.
    ///     Rounding to 15 decimal places removes that noise.
    /// </summary>
    private static decimal Trigonometric(Func<double, double> function, decimal radians)
    {
        return Math.Round(DecimalMath.FromDouble(function((double)radians)), 15);
    }

    /// <summary>
    ///     1-based position of the first occurrence of part in text, starting at start. Case-sensitive.
    ///     An empty part is found at start.
    /// </summary>
    private static decimal Find(string part, string text, decimal start)
    {
        if (start != decimal.Truncate(start) || start < 1 || start > text.Length + 1)
        {
            throw new ArithmeticException($"Start must be a whole number from 1 to {text.Length + 1}");
        }

        int index = text.IndexOf(part, (int)start - 1, StringComparison.Ordinal);

        if (index == -1)
        {
            throw new ArithmeticException("Text not found");
        }

        return index + 1;
    }

    private static int ToDecimals(decimal value)
    {
        if (value != decimal.Truncate(value) || value < 0 || value > 28)
        {
            throw new ArithmeticException("Decimal places must be a whole number from 0 to 28");
        }

        return (int)value;
    }
}
