using CincoVertice.Common.Utils.Config;
using CincoVertice.Common.Utils.Config.Tokens;
using CincoVertice.Common.Utils.Lexer;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Config.Tokens;

public class GerCommentTest
{
    [Theory]
    [InlineData("# ABC", 5, 5, '\0')]
    [InlineData(@"# ABC
Another line", 5, 7, 'A')]
    [InlineData("# This is a test", 16, 16, '\0')]
    [InlineData("# [ABC]", 7, 7, '\0')]
    [InlineData("# \"ABC\"", 7, 7, '\0')]
    public void GetComment_FindsComment_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        ConfigLexer lexer = new(testStr);

        // Act
        lexer.Char(0);
        GenericToken ret = lexer.GetComment();

        // Assert
        Assert.Equal(1, lexer.Tokens.Count);
        Assert.Equal(ConfigTokenType.Comment, lexer.Tokens.Token(0).Type);

        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(ConfigTokenType.Comment, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }
}
