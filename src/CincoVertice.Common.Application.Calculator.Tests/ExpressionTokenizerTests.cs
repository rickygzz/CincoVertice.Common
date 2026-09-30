using CincoVertice.Common.Application.Calculator.Models;
using Xunit;

namespace CincoVertice.Common.Application.Calculator.Tests;

public class ExpressionTokenizerTests
{
    public static IEnumerable<object[]> TokenizeTestData =>
        new List<object[]>
        {
            new object[] { null!,  new List<Token>() { new(TokenType.EndOfExpression, string.Empty, 0) } },
            new object[] { "",     new List<Token>() { new(TokenType.EndOfExpression, string.Empty, 0) } },
            new object[] { "1",    new List<Token>() { new(TokenType.Number, "1", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "2",    new List<Token>() { new(TokenType.Number, "2", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "3",    new List<Token>() { new(TokenType.Number, "3", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "4",    new List<Token>() { new(TokenType.Number, "4", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "5",    new List<Token>() { new(TokenType.Number, "5", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "6",    new List<Token>() { new(TokenType.Number, "6", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "7",    new List<Token>() { new(TokenType.Number, "7", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "8",    new List<Token>() { new(TokenType.Number, "8", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "9",    new List<Token>() { new(TokenType.Number, "9", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "0",    new List<Token>() { new(TokenType.Number, "0", 0), new(TokenType.EndOfExpression, string.Empty, 1) } },
            new object[] { "1.1",  new List<Token>() { new(TokenType.Number, "1.1", 0), new(TokenType.EndOfExpression, string.Empty, 3) } },
            new object[] { "1.",   new List<Token>() { new(TokenType.Number, "1.", 0), new(TokenType.EndOfExpression, string.Empty, 2) } },
            new object[] { "1..",   new List<Token>() { new(TokenType.Number, "1.", 0), new(TokenType.EndOfExpression, string.Empty, 2) } }
        };

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
}
