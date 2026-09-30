using CincoVertice.Common.Utils.Config;
using CincoVertice.Common.Utils.Config.Tokens;
using CincoVertice.Common.Utils.Lexer;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Config.Tokens;

public class GetStringTest
{
    [Theory]
    [InlineData("ABC", 3, 3, '\0')]
    [InlineData("This is a test", 14, 14, '\0')]
    [InlineData("[ABC]", 5, 5, '\0')]
    [InlineData("\"ABC\"", 5, 5, '\0')]
    public void GetString_FindsStringUnquoted_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        ConfigLexer lexer = new(testStr);

        // Act
        lexer.Char(0);
        GenericToken ret = lexer.GetString();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(ConfigTokenType.StringUnquoted, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("[\"ABC\"]", 3, 7, '\0')]
    [InlineData(@"[""ABC
DEF""]", 8, 12, '\0')]
    public void GetString_FindsQuotedString_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        ConfigLexer lexer = new(testStr);

        // Act
        lexer.Char(0);
        GenericToken ret = lexer.GetString();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(ConfigTokenType.StringQuoted, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("[\"ABC", 3, 5, '\0')]
    [InlineData(@"[""ABC
DEF", 8, 10, '\0')]
    public void GetString_FindsQuotedStringNotClosed_TokenTypeError(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        ConfigLexer lexer = new(testStr);

        // Act
        lexer.Char(0);
        GenericToken ret = lexer.GetString();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(ConfigTokenType.ErrorQuotedStringNotClosed, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }
}
