using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Extensions;

public class EqualsStringTest
{
    [Theory]
    [InlineData("ab", "a", 0)]
    [InlineData("TEXTabc", "TEXT", 0)]
    public void EqualsString_FindsString_ChangesCharIndex(string testStr, string strToSkip, int expectedNewIndex)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        bool ret = lexer.EqualsString(strToSkip);

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.True(ret);
    }

    [Theory]
    [InlineData("ab", "b", 0)]
    [InlineData("TEXTabc", "text", 0)]
    public void EqualsString_FindsString_NoChangeInCharIndex(string testStr, string strToSkip, int expectedNewIndex)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        bool ret = lexer.EqualsString(strToSkip);

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.False(ret);
    }
}
