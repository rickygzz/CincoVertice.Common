using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Extensions;

public class IsOctDigitTest
{
    [Theory]
    [InlineData("01234567")]
    public void IsOctDigit_GetsValidDigits_ReturnsTrue(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.True(lexer.IsOctDigit());

            lexer.NextChar();
        }
    }

    [Theory]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    public void IsOctDigit_GetsInvalidDigits_ReturnsFalse(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.False(lexer.IsOctDigit());

            lexer.NextChar();
        }
    }
}
