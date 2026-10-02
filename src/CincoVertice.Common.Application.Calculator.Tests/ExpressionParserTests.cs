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
