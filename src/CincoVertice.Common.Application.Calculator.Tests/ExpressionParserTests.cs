using CincoVertice.Common.Application.Calculator.Ast;
using Xunit;

namespace CincoVertice.Common.Application.Calculator.Tests;

public class ExpressionParserTests
{
    private static ExpressionNode Parse(string expression)
    {
        var tokens = new ExpressionTokenizer().Tokenize(expression);

        return new ExpressionParser().Parse(tokens);
    }

    [Theory]
    // Numbers and identifiers
    [InlineData("1", "1")]
    [InlineData("1.5", "1.5")]
    [InlineData(".5", "0.5")]
    [InlineData("1.", "1")]
    [InlineData("pi", "pi")]
    // Precedence and associativity
    [InlineData("1+2", "(+ 1 2)")]
    [InlineData("1+2*3", "(+ 1 (* 2 3))")]
    [InlineData("(1+2)*3", "(* (+ 1 2) 3)")]
    [InlineData("1-2-3", "(- (- 1 2) 3)")]
    [InlineData("8/4/2", "(/ (/ 8 4) 2)")]
    [InlineData("7%3*2", "(* (% 7 3) 2)")]
    [InlineData("2^3^2", "(^ 2 (^ 3 2))")]
    [InlineData("2*3^2", "(* 2 (^ 3 2))")]
    [InlineData("((1))", "1")]
    // Signs
    [InlineData("-2^2", "(- (^ 2 2))")]
    [InlineData("2^-1", "(^ 2 (- 1))")]
    [InlineData("--1", "(- (- 1))")]
    [InlineData("+1", "(+ 1)")]
    [InlineData("2*-3", "(* 2 (- 3))")]
    [InlineData("1--1", "(- 1 (- 1))")]
    // Function calls
    [InlineData("sqrt(2)", "(sqrt 2)")]
    [InlineData("max(1, 2+3)", "(max 1 (+ 2 3))")]
    [InlineData("rand()", "(rand)")]
    [InlineData("max(min(1, 2), -pi)", "(max (min 1 2) (- pi))")]
    // Text
    [InlineData("len(\"abc\")", "(len \"abc\")")]
    [InlineData("len(\"a\"\"b\") + 1", "(+ (len \"a\"\"b\") 1)")]
    // Unit conversions
    [InlineData("10 km to mi", "(to 10 km mi)")]
    [InlineData("10 km TO mi", "(to 10 km mi)")]
    [InlineData("(1+2)*3 km to mi", "(to (* (+ 1 2) 3) km mi)")]
    [InlineData("-1 C to F", "(to (- 1) C F)")]
    [InlineData("pi m to ft", "(to pi m ft)")]
    [InlineData("100 km/h to m/s", "(to 100 km/h m/s)")]
    [InlineData("2 m^2 to ft^2", "(to 2 m^2 ft^2)")]
    // Time conversions
    [InlineData("10:30 Mexico to India", "(time 10:30 Mexico India)")]
    [InlineData("3 pm sao paulo to new york", "(time 15:00 12h sao paulo new york)")]
    [InlineData("12:15 am cali to Bogota", "(time 00:15 12h cali Bogota)")]
    [InlineData("10:30 to China", "(time 10:30 local China)")]
    [InlineData("now to China", "(time now local China)")]
    [InlineData("9:00 America/Lima to UTC-5", "(time 09:00 America/Lima UTC-5)")]
    [InlineData("9:00 GMT+5:30 to utc", "(time 09:00 GMT+5:30 utc)")]
    public void ExpressionParser_ValidExpression_BuildsTree(string expression, string expectedTree)
    {
        ExpressionNode node = Parse(expression);

        Assert.Equal(expectedTree, node.ToString());
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("   ", 3)]
    [InlineData("1+", 2)]
    [InlineData("*1", 0)]
    [InlineData("(1", 2)]
    [InlineData("1)", 1)]
    [InlineData("()", 1)]
    [InlineData("1 2", 2)]
    [InlineData("2pi", 1)]
    [InlineData("max(1,)", 6)]
    [InlineData("max(1 2)", 6)]
    [InlineData("max(1", 5)]
    [InlineData("10 km", 3)]
    [InlineData("10 km mi", 3)]
    [InlineData("10 km/", 3)]
    [InlineData("10 to mi", 3)]
    [InlineData("10 km to", 8)]
    [InlineData("10 km to 5", 9)]
    [InlineData("10 km to mi 5", 12)]
    [InlineData("(10 km to mi)", 4)]
    [InlineData("10 km to mi + 1", 12)]
    public void ExpressionParser_InvalidExpression_ThrowsWithPosition(string expression, int expectedPosition)
    {
        var exception = Assert.Throws<ExpressionException>(() => Parse(expression));

        Assert.Equal(expectedPosition, exception.Position);
    }

    [Fact]
    public void ExpressionParser_BinaryNode_HasOperatorPosition()
    {
        var node = Assert.IsType<BinaryNode>(Parse("1 + 2"));

        Assert.Equal(BinaryOperator.Add, node.Operator);
        Assert.Equal(2, node.Position);
    }

    [Fact]
    public void ExpressionParser_ReusedInstance_ParsesIndependently()
    {
        var tokenizer = new ExpressionTokenizer();
        var parser = new ExpressionParser();
        parser.Parse(tokenizer.Tokenize("1+2*3"));

        ExpressionNode node = parser.Parse(tokenizer.Tokenize("4"));

        Assert.Equal("4", node.ToString());
    }
}
