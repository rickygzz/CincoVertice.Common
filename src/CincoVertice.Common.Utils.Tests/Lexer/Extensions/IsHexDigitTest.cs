using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Extensions;

public class IsHexDigitTest
{
    [Theory]
    [InlineData("1234567890ABCDEFabcdef")]
    public void IsHexDigit_GetsValidDigits_ReturnsTrue(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.True(lexer.IsHexDigit());

            lexer.NextChar();
        }
    }

    [Theory]
    [InlineData("GHIJKLMNOPQRSTUVWXYZ")]
    [InlineData("ghijklmnopqrstuvwxyz")]
    public void IsHexDigit_GetsInvalidDigits_ReturnsFalse(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.False(lexer.IsHexDigit());

            lexer.NextChar();
        }
    }
}
