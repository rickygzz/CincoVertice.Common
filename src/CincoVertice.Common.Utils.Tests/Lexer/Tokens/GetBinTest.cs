using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Tokens;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Tokens;

public class GetBinTest
{
    [Theory]
    [InlineData("012345678", 2, 2, '2')]
    [InlineData("1110A", 4, 4, 'A')]
    public void GetBin_FindsBin_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetBin();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(GenericTokenType.Binary, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData(".10", 0, '.')]
    [InlineData("-.10", 0, '-')]
    [InlineData(".01", 0, '.')]
    [InlineData("-10", 0, '-')]
    [InlineData("-A", 0, '-')]
    [InlineData("-.", 0, '-')]
    [InlineData("-.A", 0, '-')]
    [InlineData(".A", 0, '.')]
    public void GetBin_FindsNoBin_NoChangesInCharIndex(
        string testStr,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetBin();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(0, ret.Length);
        Assert.Equal(GenericTokenType.Null, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }
}
