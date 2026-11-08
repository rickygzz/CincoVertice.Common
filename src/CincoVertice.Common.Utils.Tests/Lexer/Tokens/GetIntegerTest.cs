using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Tokens;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Tokens;

public class GetIntegerTest
{
    [Theory]
    [InlineData("12", 2, 2, '\0')]
    [InlineData("12.50", 2, 2, '.')]
    [InlineData("12.", 2, 2, '.')]
    [InlineData("12A", 2, 2, 'A')]
    [InlineData("-12", 3, 3, '\0')]
    public void GetInteger_FindsInteger_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetInteger();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(GenericTokenType.Integer, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData(".12", 0, '.')]
    [InlineData("-.12", 0, '-')]
    [InlineData(".0", 0, '.')]
    [InlineData("-A", 0, '-')]
    [InlineData("-.", 0, '-')]
    [InlineData("-.A", 0, '-')]
    [InlineData(".A", 0, '.')]
    public void GetInteger_FindsNoInteger_NoChangesInCharIndex(
        string testStr,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetInteger();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(0, ret.Length);
        Assert.Equal(GenericTokenType.Null, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }
}
