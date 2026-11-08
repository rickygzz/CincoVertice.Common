using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Extensions;

public class IsBlankCharTest
{
    [Theory]
    [InlineData(" \t\0xD\0xC")]
    public void IsBlankChar_GetsValidBlankChar_ReturnsTrue(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.True(lexer.IsBlankChar());

            lexer.NextChar();
        }
    }

    [Theory]
    [InlineData("0123456789")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    public void IsBlankChar_GetsInvalidBlankChar_ReturnsFalse(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.False(lexer.IsBlankChar());

            lexer.NextChar();
        }
    }
}
