using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Tokens;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Tokens;

public class GetOctTest
{
    [Theory]
    [InlineData("012345678", 8, 8, '8')]
    [InlineData("777A", 3, 3, 'A')]
    public void GetOct_FindsOct_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetOct();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(GenericTokenType.Octal, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData(".12", 0, '.')]
    [InlineData("-.12", 0, '-')]
    [InlineData(".0", 0, '.')]
    [InlineData("-12", 0, '-')]
    [InlineData("-A", 0, '-')]
    [InlineData("-.", 0, '-')]
    [InlineData("-.A", 0, '-')]
    [InlineData(".A", 0, '.')]
    public void GetOct_FindsNoOct_NoChangesInCharIndex(
        string testStr,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetOct();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(0, ret.Length);
        Assert.Equal(GenericTokenType.Null, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }
}
