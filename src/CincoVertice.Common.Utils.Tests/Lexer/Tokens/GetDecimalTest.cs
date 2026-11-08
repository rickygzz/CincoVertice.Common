using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Tokens;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Tokens;

public class GetDecimalTest
{
    [Theory]
    [InlineData("12.50", 5, 5, '\0')]
    [InlineData("12", 2, 2, '\0')]
    [InlineData("12.", 3, 3, '\0')]
    [InlineData("-12", 3, 3, '\0')]
    [InlineData(".12", 3, 3, '\0')]
    [InlineData("-.12", 4, 4, '\0')]
    [InlineData(".0", 2, 2, '\0')]
    [InlineData("-.01", 4, 4, '\0')]
    [InlineData("12.50t", 5, 5, 't')]
    [InlineData("12t", 2, 2, 't')]
    [InlineData("12.t", 3, 3, 't')]
    [InlineData("-12t", 3, 3, 't')]
    [InlineData(".12t", 3, 3, 't')]
    [InlineData("-.12t", 4, 4, 't')]
    [InlineData(".0t", 2, 2, 't')]
    [InlineData("-.01t", 4, 4, 't')]
    public void GetDecimal_FindsDecimal_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetDecimal();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(GenericTokenType.Decimal, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("-A", 0, '-')]
    [InlineData("-.", 0, '-')]
    [InlineData("-.A", 0, '-')]
    [InlineData(".A", 0, '.')]
    public void GetDecimal_FindsNoDecimal_NoChangesInCharIndex(
        string testStr,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetDecimal();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(0, ret.Length);
        Assert.Equal(GenericTokenType.Null, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }
}
