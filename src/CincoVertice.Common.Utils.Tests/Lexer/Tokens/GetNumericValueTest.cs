using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Tokens;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Tokens;

public class GetNumericValueTest
{
    [Theory]
    [InlineData("0b012345678", 4, 4, '2')]
    [InlineData("0b1110A", 6, 6, 'A')]
    public void GetValue_FindsBin_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetNumericValue();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(GenericTokenType.Binary, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("0b2", 1, 1, 'b')]
    [InlineData("0bg", 1, 1, 'b')]
    [InlineData("-0b.", 2, 2, 'b')]
    public void GetValue_FindsNoBin_Returns0Integer(
        string testStr,
        int expectedNewIndex,
        int expectedNewLength,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetNumericValue();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedNewLength, ret.Length);
        Assert.Equal(GenericTokenType.Integer, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("0x12.", 4, 4, '.')]
    [InlineData("0x12.20", 4, 4, '.')]
    [InlineData("0x12A", 5, 5, '\0')]
    [InlineData("0x12a", 5, 5, '\0')]
    [InlineData("0xFF", 4, 4, '\0')]
    [InlineData("0xFFG", 4, 4, 'G')]
    public void GetValue_FindsHex_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetNumericValue();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(GenericTokenType.Hex, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("0x.", 1, 1, 'x')]
    [InlineData("0xg", 1, 1, 'x')]
    [InlineData("-0x.", 2, 2, 'x')]
    public void GetHex_FindsNoHex_NoChangesInCharIndex(
        string testStr,
        int expectedNewIndex,
        int expectedNewLength,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetNumericValue();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedNewLength, ret.Length);
        Assert.Equal(GenericTokenType.Integer, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("0.15", 4, 4, '\0')]
    [InlineData("-0.1G", 4, 4, 'G')]
    [InlineData("-.15", 4, 4, '\0')]
    [InlineData("-.15G", 4, 4, 'G')]
    [InlineData("-10.G", 4, 4, 'G')]
    [InlineData("12.50", 5, 5, '\0')]
    [InlineData("12.5A", 4, 4, 'A')]
    [InlineData("1.A", 2, 2, 'A')]
    public void GetValue_FindsDecimal_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetNumericValue();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(GenericTokenType.Decimal, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("-10G", 3, 3, 'G')]
    public void GetValue_FindsInteger_ChangesCharIndex(
        string testStr,
        int expectedLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        GenericToken ret = lexer.GetNumericValue();

        // Assert
        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedLength, ret.Length);
        Assert.Equal(GenericTokenType.Integer, ret.Type);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }
}
