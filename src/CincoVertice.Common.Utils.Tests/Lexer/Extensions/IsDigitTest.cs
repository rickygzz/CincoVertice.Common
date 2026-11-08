using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Extensions;

public class IsDigitTest
{
    [Theory]
    [InlineData("1234567890")]
    public void IsDigit_GetsValidDigits_ReturnsTrue(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.True(lexer.IsDigit());

            lexer.NextChar();
        }
    }

    [Theory]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    public void IsDigit_GetsInvalidDigits_ReturnsFalse(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.False(lexer.IsDigit());

            lexer.NextChar();
        }
    }
}
