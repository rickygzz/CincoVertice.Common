using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Extensions;

public class SkipDigitsTest
{
    [Theory]
    [InlineData("1234", 4, '\0')]
    [InlineData("123a", 3, 'a')]
    [InlineData("1234567890a", 10, 'a')]
    public void SkipDigits_FindsDigits_ChangesCharIndex(
        string testStr,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        bool ret = lexer.SkipDigits();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.True(ret);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("a1234", 0, 'a')]
    [InlineData("a123", 0, 'a')]
    [InlineData("a1234567890", 0, 'a')]
    public void SkipDigits_FindsNoDigits_NoChangesInCharIndex(
        string testStr,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        bool ret = lexer.SkipDigits();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.False(ret);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }
}
