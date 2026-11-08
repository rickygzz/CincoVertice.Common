using System.Globalization;
using Xunit;

namespace CincoVertice.Common.Application.Calculator.Tests;

public class ExpressionCalculatorTests
{
    private readonly ExpressionCalculator _calculator = new();

    [Theory]
    // Arithmetic and precedence
    [InlineData("1+2", "3")]
    [InlineData("2+3*4", "14")]
    [InlineData("(1+2)*3", "9")]
    [InlineData("10-4-3", "3")]
    [InlineData("10/4", "2.5")]
    [InlineData("7%3", "1")]
    [InlineData("-7%3", "-1")]
    [InlineData("1.5*2", "3")]
    // decimal is exact for decimal fractions
    [InlineData("0.1+0.2", "0.3")]
    // Signs and power
    [InlineData("-2^2", "-4")]
    [InlineData("(-2)^2", "4")]
    [InlineData("2^-1", "0.5")]
    [InlineData("2^3^2", "512")]
    [InlineData("2^10", "1024")]
    [InlineData("0^0", "1")]
    [InlineData("1.1^2", "1.21")]
    [InlineData("--3", "3")]
    // Constants (case-insensitive)
    [InlineData("pi", "3.1415926535897932384626433833")]
    [InlineData("PI", "3.1415926535897932384626433833")]
    [InlineData("e", "2.7182818284590452353602874714")]
    // Functions
    [InlineData("abs(-3)", "3")]
    [InlineData("sqrt(16)", "4")]
    [InlineData("sqrt(0)", "0")]
    [InlineData("round(2.5)", "3")]
    [InlineData("round(-2.5)", "-3")]
    [InlineData("round(2.345, 2)", "2.35")]
    [InlineData("floor(-1.5)", "-2")]
    [InlineData("ceil(1.2)", "2")]
    [InlineData("min(3, 1, 2)", "1")]
    [InlineData("max(3, 1, 2)", "3")]
    [InlineData("max(5)", "5")]
    [InlineData("log(1000)", "3")]
    [InlineData("exp(0)", "1")]
    [InlineData("sin(0)", "0")]
    [InlineData("cos(0)", "1")]
    [InlineData("SQRT(4) + Max(1, 2)", "4")]
    // Text
    [InlineData("len(\"hello\")", "5")]
    [InlineData("len(\"\")", "0")]
    [InlineData("len(\"a b\")", "3")]
    [InlineData("len(\"a\"\"b\")", "3")]
    [InlineData("LEN(\"hello\") * 2 + 1", "11")]
    [InlineData("find(\"b\", \"abc\")", "2")]
    [InlineData("find(\"a\", \"banana\")", "2")]
    [InlineData("find(\"a\", \"banana\", 3)", "4")]
    [InlineData("find(\"a\", \"banana\", 2)", "2")]
    [InlineData("find(\"na\", \"banana\") + 1", "4")]
    [InlineData("find(\"\", \"abc\")", "1")]
    [InlineData("find(\"\", \"abc\", 4)", "4")]
    [InlineData("find(\"\"\"\", \"say \"\"hi\"\"\")", "5")]
    public void Evaluate_ValidExpression_ReturnsValue(string expression, string expected)
    {
        decimal result = _calculator.Evaluate(expression);

        Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [Fact]
    public void Evaluate_Sqrt_HasFullDecimalPrecision()
    {
        decimal result = _calculator.Evaluate("sqrt(2)");

        Assert.Equal(1.4142135623730950488016887242m, result);
    }

    [Fact]
    public void Evaluate_FractionalPower_UsesDoublePrecision()
    {
        decimal result = _calculator.Evaluate("2^0.5");

        Assert.Equal(1.41421356237310m, result, 14);
    }

    [Theory]
    [InlineData("1/0", 1, "Division by zero")]
    [InlineData("1 + 5 % 0", 6, "Division by zero")]
    [InlineData("0^-1", 1, "Division by zero")]
    [InlineData("10^30", 2, "Number is too large")]
    [InlineData("79228162514264337593543950335 + 1", 30, "Number is too large")]
    [InlineData("(-8)^0.5", 4, "Result is not a real number")]
    [InlineData("sqrt(-1)", 0, "Square root of a negative number")]
    [InlineData("ln(0)", 0, "Logarithm of zero or a negative number")]
    [InlineData("log(-1)", 0, "Logarithm of zero or a negative number")]
    [InlineData("round(1.5, 0.5)", 0, "Decimal places must be a whole number from 0 to 28")]
    [InlineData("round(1.5, 29)", 0, "Decimal places must be a whole number from 0 to 28")]
    [InlineData("2 * x", 4, "Unknown name 'x'")]
    [InlineData("sqrt", 0, "Unknown name 'sqrt'")]
    [InlineData("1 + foo(1)", 4, "Unknown function 'foo'")]
    [InlineData("pi()", 0, "Unknown function 'pi'")]
    [InlineData("sqrt(1, 2)", 0, "Function 'sqrt' expects 1 argument")]
    [InlineData("round(1, 2, 3)", 0, "Function 'round' expects 1 to 2 arguments")]
    [InlineData("min()", 0, "Function 'min' expects at least 1 argument")]
    [InlineData("\"abc\"", 0, "Text can only be used as a function argument")]
    [InlineData("1 + \"a\"", 4, "Text can only be used as a function argument")]
    [InlineData("-\"a\"", 1, "Text can only be used as a function argument")]
    [InlineData("len(5)", 4, "Expected text")]
    [InlineData("len(pi)", 4, "Expected text")]
    [InlineData("sqrt(\"4\")", 5, "Expected a number")]
    [InlineData("max(1, \"2\")", 7, "Expected a number")]
    [InlineData("len(\"a\", \"b\")", 0, "Function 'len' expects 1 argument")]
    [InlineData("len()", 0, "Function 'len' expects 1 argument")]
    [InlineData("find(\"x\", \"abc\")", 0, "Text not found")]
    [InlineData("find(\"B\", \"abc\")", 0, "Text not found")]
    [InlineData("find(\"a\", \"banana\", 7)", 0, "Text not found")]
    [InlineData("find(\"a\", \"abc\", 0)", 0, "Start must be a whole number from 1 to 4")]
    [InlineData("find(\"a\", \"abc\", 5)", 0, "Start must be a whole number from 1 to 4")]
    [InlineData("find(\"a\", \"abc\", 1.5)", 0, "Start must be a whole number from 1 to 4")]
    [InlineData("find(1, \"abc\")", 5, "Expected text")]
    [InlineData("find(\"a\", \"abc\", \"1\")", 17, "Expected a number")]
    [InlineData("find(\"a\")", 0, "Function 'find' expects 2 to 3 arguments")]
    public void Evaluate_InvalidCalculation_ThrowsWithPosition(string expression, int expectedPosition, string expectedMessage)
    {
        var exception = Assert.Throws<ExpressionException>(() => _calculator.Evaluate(expression));

        Assert.Equal(expectedPosition, exception.Position);
        Assert.StartsWith(expectedMessage + " at position", exception.Message);
    }

    [Theory]
    [InlineData("1.5*2", "3")]
    [InlineData("0.1+0.2", "0.3")]
    [InlineData("1.10", "1.1")]
    [InlineData("-0", "0")]
    [InlineData("-0.5", "-0.5")]
    [InlineData("1/3", "0.3333333333333333333333333333")]
    [InlineData("10^20", "100000000000000000000")]
    [InlineData("10^-20", "0.00000000000000000001")]
    [InlineData("79228162514264337593543950335", "79228162514264337593543950335")]
    [InlineData("sin(pi)", "0")]
    [InlineData("cos(pi/2)", "0")]
    [InlineData("cos(pi)", "-1")]
    [InlineData("sin(pi/6)", "0.5")]
    public void Format_Result_ShowsPlainNumber(string expression, string expected)
    {
        string text = ExpressionCalculator.Format(_calculator.Evaluate(expression));

        Assert.Equal(expected, text);
    }

    [Theory]
    [InlineData("1", "1")]
    [InlineData("999", "999")]
    [InlineData("1000", "1,000")]
    [InlineData("1234567.5", "1,234,567.5")]
    [InlineData("-1234567.5", "-1,234,567.5")]
    [InlineData("0.0001", "0.0001")]
    [InlineData("-0", "0")]
    [InlineData("1/3", "0.3333333333333333333333333333")]
    [InlineData("79228162514264337593543950335", "79,228,162,514,264,337,593,543,950,335")]
    public void Format_GroupThousands_SeparatesWithComma(string expression, string expected)
    {
        string text = ExpressionCalculator.Format(_calculator.Evaluate(expression), groupThousands: true);

        Assert.Equal(expected, text);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("1 +", 3)]
    [InlineData("1 $ 2", 2)]
    public void Evaluate_InvalidSyntax_ThrowsWithPosition(string expression, int expectedPosition)
    {
        var exception = Assert.Throws<ExpressionException>(() => _calculator.Evaluate(expression));

        Assert.Equal(expectedPosition, exception.Position);
    }
}
