namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Math functions for decimal, which System.Math does not provide.
///     <para>Errors are thrown as <see cref="ArithmeticException"/> (or its subclasses), which the evaluator
///     reports at the position of the operator or function.</para>
/// </summary>
public static class DecimalMath
{
    /// <summary>
    ///     x^y. Whole exponents are exact (repeated squaring); fractional exponents use double (about 15 digits).
    /// </summary>
    public static decimal Pow(decimal x, decimal y)
    {
        if (y != decimal.Truncate(y) || y > long.MaxValue || y < -long.MaxValue)
        {
            return FromDouble(Math.Pow((double)x, (double)y));
        }

        long exponent = (long)y;

        if (exponent < 0)
        {
            // 0^-n throws DivideByZeroException. Using 1/x lets tiny results round to 0 instead of overflowing.
            x = 1 / x;
            exponent = -exponent;
        }

        decimal result = 1;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result *= x;
            }

            exponent >>= 1;

            if (exponent > 0)
            {
                x *= x;
            }
        }

        return result;
    }

    /// <summary>
    ///     Square root with full decimal precision: a double estimate refined with Newton's method.
    /// </summary>
    public static decimal Sqrt(decimal x)
    {
        if (x < 0)
        {
            throw new ArithmeticException("Square root of a negative number");
        }

        if (x == 0)
        {
            return 0;
        }

        decimal guess = (decimal)Math.Sqrt((double)x);

        for (int i = 0; i < 10; i++)
        {
            decimal next = (guess + x / guess) / 2;

            if (next == guess)
            {
                break;
            }

            guess = next;
        }

        return guess;
    }

    /// <summary>Natural logarithm (double precision).</summary>
    public static decimal Ln(decimal x)
    {
        EnsurePositive(x);

        return FromDouble(Math.Log((double)x));
    }

    /// <summary>Base-10 logarithm (double precision).</summary>
    public static decimal Log10(decimal x)
    {
        EnsurePositive(x);

        return FromDouble(Math.Log10((double)x));
    }

    /// <summary>
    ///     Converts a double result to decimal (keeps about 15 significant digits).
    /// </summary>
    public static decimal FromDouble(double value)
    {
        if (double.IsNaN(value))
        {
            throw new ArithmeticException("Result is not a real number");
        }

        if (double.IsInfinity(value))
        {
            throw new OverflowException();
        }

        // Throws OverflowException if outside the decimal range
        return (decimal)value;
    }

    private static void EnsurePositive(decimal x)
    {
        if (x <= 0)
        {
            throw new ArithmeticException("Logarithm of zero or a negative number");
        }
    }
}
