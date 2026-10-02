using CincoVertice.Common.Application.Calculator.Models;
using Xunit;

namespace CincoVertice.Common.Application.Calculator.Tests;

public class ExpressionTokenizerTests
{
    public static IEnumerable<object[]> TokenizeTestData =>
        [
            [null!,  new List<Token>() { new(TokenType.EndOfExpression, string.Empty, 0) }],
            ["",     new List<Token>() { new(TokenType.EndOfExpression, string.Empty, 0) }],
            ["1",    new List<Token>() { new(TokenType.Number, "1", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["2",    new List<Token>() { new(TokenType.Number, "2", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["3",    new List<Token>() { new(TokenType.Number, "3", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["4",    new List<Token>() { new(TokenType.Number, "4", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["5",    new List<Token>() { new(TokenType.Number, "5", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["6",    new List<Token>() { new(TokenType.Number, "6", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["7",    new List<Token>() { new(TokenType.Number, "7", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["8",    new List<Token>() { new(TokenType.Number, "8", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["9",    new List<Token>() { new(TokenType.Number, "9", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["0",    new List<Token>() { new(TokenType.Number, "0", 0), new(TokenType.EndOfExpression, string.Empty, 1) }],
            ["1.1",  new List<Token>() { new(TokenType.Number, "1.1", 0), new(TokenType.EndOfExpression, string.Empty, 3) }],
            ["1.",   new List<Token>() { new(TokenType.Number, "1.", 0), new(TokenType.EndOfExpression, string.Empty, 2) }]
        ];

    [Theory]
    [MemberData(nameof(TokenizeTestData))]
    public void ExpressionTokenizer_ShouldTokenizeExpressionCorrectly(string? expression, List<Token> expectedTokens)
    {
        // Arrange
        var tokenizer = new ExpressionTokenizer();

        // Act
        var tokens = tokenizer.Tokenize(expression);

        // Assert
        Assert.Equal(expectedTokens.Count, tokens.Count);

        for (int i = 0; i < expectedTokens.Count; i++)
        {
            Assert.Equal(expectedTokens[i], tokens[i]);
        }
    }

    public static IEnumerable<object[]> TokenizeOperatorsTestData =>
        [
            ["1+2",  new List<Token>() { new(TokenType.Number, "1", 0), new(TokenType.Plus, "+", 1), new(TokenType.Number, "2", 2), new(TokenType.EndOfExpression, string.Empty, 3) }],
            ["3-2",  new List<Token>() { new(TokenType.Number, "3", 0), new(TokenType.Minus, "-", 1), new(TokenType.Number, "2", 2), new(TokenType.EndOfExpression, string.Empty, 3) }],
            ["-2",   new List<Token>() { new(TokenType.Minus, "-", 0), new(TokenType.Number, "2", 1), new(TokenType.EndOfExpression, string.Empty, 2) }],
            ["2*3",  new List<Token>() { new(TokenType.Number, "2", 0), new(TokenType.Multiply, "*", 1), new(TokenType.Number, "3", 2), new(TokenType.EndOfExpression, string.Empty, 3) }],
            ["6/3",  new List<Token>() { new(TokenType.Number, "6", 0), new(TokenType.Divide, "/", 1), new(TokenType.Number, "3", 2), new(TokenType.EndOfExpression, string.Empty, 3) }],
            ["7%3",  new List<Token>() { new(TokenType.Number, "7", 0), new(TokenType.Modulo, "%", 1), new(TokenType.Number, "3", 2), new(TokenType.EndOfExpression, string.Empty, 3) }],
            ["2^3",  new List<Token>() { new(TokenType.Number, "2", 0), new(TokenType.Power, "^", 1), new(TokenType.Number, "3", 2), new(TokenType.EndOfExpression, string.Empty, 3) }],
            ["(1)",  new List<Token>() { new(TokenType.LeftParenthesis, "(", 0), new(TokenType.Number, "1", 1), new(TokenType.RightParenthesis, ")", 2), new(TokenType.EndOfExpression, string.Empty, 3) }],
            [".5",   new List<Token>() { new(TokenType.Number, ".5", 0), new(TokenType.EndOfExpression, string.Empty, 2) }],
        ];

    [Theory]
    [MemberData(nameof(TokenizeOperatorsTestData))]
    public void ExpressionTokenizer_ShouldTokenizeOperators(string expression, List<Token> expectedTokens)
    {
        var tokens = new ExpressionTokenizer().Tokenize(expression);

        Assert.Equal(expectedTokens, tokens);
    }

    public static IEnumerable<object[]> TokenizeWhitespaceTestData =>
        [
            ["   ",      new List<Token>() { new(TokenType.EndOfExpression, string.Empty, 3) }],
            [" 1",       new List<Token>() { new(TokenType.Number, "1", 1), new(TokenType.EndOfExpression, string.Empty, 2) }],
            ["1 ",       new List<Token>() { new(TokenType.Number, "1", 0), new(TokenType.EndOfExpression, string.Empty, 2) }],
            ["1 + 2",    new List<Token>() { new(TokenType.Number, "1", 0), new(TokenType.Plus, "+", 2), new(TokenType.Number, "2", 4), new(TokenType.EndOfExpression, string.Empty, 5) }],
            ["\t1\r\n",  new List<Token>() { new(TokenType.Number, "1", 1), new(TokenType.EndOfExpression, string.Empty, 4) }],
        ];

    [Theory]
    [MemberData(nameof(TokenizeWhitespaceTestData))]
    public void ExpressionTokenizer_ShouldSkipWhitespace(string expression, List<Token> expectedTokens)
    {
        var tokens = new ExpressionTokenizer().Tokenize(expression);

        Assert.Equal(expectedTokens, tokens);
    }

    public static IEnumerable<object[]> TokenizeIdentifiersTestData =>
        [
            ["pi",        new List<Token>() { new(TokenType.Identifier, "pi", 0), new(TokenType.EndOfExpression, string.Empty, 2) }],
            ["_x1",       new List<Token>() { new(TokenType.Identifier, "_x1", 0), new(TokenType.EndOfExpression, string.Empty, 3) }],
            ["2pi",       new List<Token>() { new(TokenType.Number, "2", 0), new(TokenType.Identifier, "pi", 1), new(TokenType.EndOfExpression, string.Empty, 3) }],
            ["sqrt(2)",   new List<Token>() { new(TokenType.Identifier, "sqrt", 0), new(TokenType.LeftParenthesis, "(", 4), new(TokenType.Number, "2", 5), new(TokenType.RightParenthesis, ")", 6), new(TokenType.EndOfExpression, string.Empty, 7) }],
            ["max(1, 2)", new List<Token>() { new(TokenType.Identifier, "max", 0), new(TokenType.LeftParenthesis, "(", 3), new(TokenType.Number, "1", 4), new(TokenType.Comma, ",", 5), new(TokenType.Number, "2", 7), new(TokenType.RightParenthesis, ")", 8), new(TokenType.EndOfExpression, string.Empty, 9) }],
        ];

    [Theory]
    [MemberData(nameof(TokenizeIdentifiersTestData))]
    public void ExpressionTokenizer_ShouldTokenizeIdentifiers(string expression, List<Token> expectedTokens)
    {
        var tokens = new ExpressionTokenizer().Tokenize(expression);

        Assert.Equal(expectedTokens, tokens);
    }

    public static IEnumerable<object[]> TokenizeStringsTestData =>
        [
            ["\"abc\"",        new List<Token>() { new(TokenType.String, "abc", 0), new(TokenType.EndOfExpression, string.Empty, 5) }],
            ["\"\"",           new List<Token>() { new(TokenType.String, "", 0), new(TokenType.EndOfExpression, string.Empty, 2) }],
            ["\"a\"\"b\"",     new List<Token>() { new(TokenType.String, "a\"b", 0), new(TokenType.EndOfExpression, string.Empty, 6) }],
            ["\"\"\"\"",       new List<Token>() { new(TokenType.String, "\"", 0), new(TokenType.EndOfExpression, string.Empty, 4) }],
            ["\" a+b, (c) \"", new List<Token>() { new(TokenType.String, " a+b, (c) ", 0), new(TokenType.EndOfExpression, string.Empty, 12) }],
            ["len(\"hi\")",    new List<Token>() { new(TokenType.Identifier, "len", 0), new(TokenType.LeftParenthesis, "(", 3), new(TokenType.String, "hi", 4), new(TokenType.RightParenthesis, ")", 8), new(TokenType.EndOfExpression, string.Empty, 9) }],
        ];

    [Theory]
    [MemberData(nameof(TokenizeStringsTestData))]
    public void ExpressionTokenizer_ShouldTokenizeStrings(string expression, List<Token> expectedTokens)
    {
        var tokens = new ExpressionTokenizer().Tokenize(expression);

        Assert.Equal(expectedTokens, tokens);
    }

    [Theory]
    [InlineData("1 $ 2", 2)]
    [InlineData("\"abc", 0)]
    [InlineData("len(\"abc)", 4)]
    [InlineData("\"a\"\"", 0)]
    [InlineData("1.2.3", 3)]
    [InlineData(".", 0)]
    [InlineData("2 + .", 4)]
    [InlineData("é", 0)]
    [InlineData("1 + 79228162514264337593543950336", 4)]
    public void ExpressionTokenizer_InvalidInput_ThrowsWithPosition(string expression, int expectedPosition)
    {
        var tokenizer = new ExpressionTokenizer();

        var exception = Assert.Throws<ExpressionException>(() => tokenizer.Tokenize(expression));

        Assert.Equal(expectedPosition, exception.Position);
    }

    [Fact]
    public void ExpressionTokenizer_ReusedInstance_DoesNotKeepPreviousPosition()
    {
        var tokenizer = new ExpressionTokenizer();
        tokenizer.Tokenize("123");

        var tokens = tokenizer.Tokenize("");

        Assert.Equal([new Token(TokenType.EndOfExpression, string.Empty, 0)], tokens);
    }
}
