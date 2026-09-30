using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Extensions;

public class SkipGivenStringTest
{
    [Theory]
    [InlineData("ab", "a", 1, 'b')]
    [InlineData("ab", "ab", 2, '\0')]
    [InlineData("TEXTabc", "TEXT", 4, 'a')]
    public void SkipGivenString_FindsString_ChangesCharIndex(
        string testStr,
        string strToSkip,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        bool ret = lexer.SkipGivenString(strToSkip);

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.True(ret);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("ab", "b", 0, 'a')]
    [InlineData("ab", "abc", 0, 'a')]
    [InlineData("TEXTabc", "text", 0, 'T')]
    public void SkipGivenString_FindsString_NoChangeInCharIndex(
        string testStr,
        string strToSkip,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        bool ret = lexer.SkipGivenString(strToSkip);

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.False(ret);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }
}
