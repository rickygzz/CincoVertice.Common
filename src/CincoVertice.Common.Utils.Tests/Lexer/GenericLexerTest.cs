using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Models;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer;

public class GenericLexerTest
{
    [Theory]
    [InlineData("abc", 'a')]
    [InlineData("test string", 't')]
    public void CharIndexCurrentChar_InitialSetup(string testStr, char initialChar)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        Assert.Equal(0, lexer.CharIndex);
        Assert.Equal(initialChar, lexer.CurrentChar);
        Assert.Equal(testStr, lexer.Text);
        Assert.Equal(testStr.Length, lexer.TextLength);
    }

    [Theory]
    [InlineData("abc", 1, 2, "bc")]
    public void Substring_GetsValidParams_ReturnsSubString(string testStr, int startIndex, int length, string expectedResult)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        string result = lexer.Substring(startIndex, length);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("abc", 4, 1, "startIndex cannot be larger than length of string")]
    [InlineData("abc", 1, 3, "Index and length must refer to a location within the string")]
    public void Substring_GetsInvalidIndexAndLength_ThrowsOurOfRangeException(
        string testStr,
        int startIndex,
        int length,
        string expectedError)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lexer.Substring(startIndex, length);
        });

        // Act and assert
        Assert.StartsWith(expectedError, exception.Message);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("test string")]
    public void NextChar_TestLexerText(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Act and assert
        foreach (char c in testStr)
        {
            Assert.Equal(c, lexer.CurrentChar);
            lexer.NextChar();
        }

        Assert.Equal('\0', lexer.CurrentChar);
        Assert.Equal(testStr.Length, lexer.CharIndex);

        lexer.NextChar();
        Assert.Equal('\0', lexer.CurrentChar);
        Assert.Equal(testStr.Length, lexer.CharIndex);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("abcd")]
    public void PrevChar_TestLexerText(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);
        lexer.Char(testStr.Length - 1);

        // Act and assert
        for (int i = testStr.Length - 1; i >= 0; i--)
        {
            Assert.Equal(testStr[i], lexer.CurrentChar);
            lexer.PrevChar();
        }

        Assert.Equal('\0', lexer.CurrentChar);
        Assert.Equal(testStr.Length, lexer.CharIndex);

        lexer.PrevChar();
        Assert.Equal(testStr[testStr.Length - 1], lexer.CurrentChar);
        Assert.Equal(testStr.Length - 1, lexer.CharIndex);
    }

    [Fact]
    public void Position_FirstTest()
    {
        // Arrange
        GenericLexer lexer = new("abc\ndef\nghi");

        // Act & Assert
        Position pos = lexer.Position(0);
        Assert.Equal(1, pos.Line);
        Assert.Equal(1, pos.Column);

        pos = lexer.Position(1);
        Assert.Equal(1, pos.Line);
        Assert.Equal(2, pos.Column);

        pos = lexer.Position(2);
        Assert.Equal(1, pos.Line);
        Assert.Equal(3, pos.Column);

        pos = lexer.Position(3);
        Assert.Equal(1, pos.Line);
        Assert.Equal(4, pos.Column);

        pos = lexer.Position(4);
        Assert.Equal(2, pos.Line);
        Assert.Equal(1, pos.Column);

        pos = lexer.Position(5);
        Assert.Equal(2, pos.Line);
        Assert.Equal(2, pos.Column);

        pos = lexer.Position(6);
        Assert.Equal(2, pos.Line);
        Assert.Equal(3, pos.Column);

        pos = lexer.Position(7);
        Assert.Equal(2, pos.Line);
        Assert.Equal(4, pos.Column);

        pos = lexer.Position(8);
        Assert.Equal(3, pos.Line);
        Assert.Equal(1, pos.Column);

        pos = lexer.Position(9);
        Assert.Equal(3, pos.Line);
        Assert.Equal(2, pos.Column);

        pos = lexer.Position(10);
        Assert.Equal(3, pos.Line);
        Assert.Equal(3, pos.Column);

        pos = lexer.Position(11);
        Assert.Equal(3, pos.Line);
        Assert.Equal(4, pos.Column);
    }

    [Fact]
    public void Position_SecondTest()
    {
        // Arrange
        GenericLexer lexer = new("abc\r\ndef\r\nghi");

        // Act & Assert
        Position pos = lexer.Position(0);
        Assert.Equal(1, pos.Line);
        Assert.Equal(1, pos.Column);

        pos = lexer.Position(1);
        Assert.Equal(1, pos.Line);
        Assert.Equal(2, pos.Column);

        pos = lexer.Position(2);
        Assert.Equal(1, pos.Line);
        Assert.Equal(3, pos.Column);

        pos = lexer.Position(3);
        Assert.Equal(1, pos.Line);
        Assert.Equal(4, pos.Column);

        pos = lexer.Position(4);
        Assert.Equal(1, pos.Line);
        Assert.Equal(5, pos.Column);

        pos = lexer.Position(5);
        Assert.Equal(2, pos.Line);
        Assert.Equal(1, pos.Column);

        pos = lexer.Position(6);
        Assert.Equal(2, pos.Line);
        Assert.Equal(2, pos.Column);

        pos = lexer.Position(7);
        Assert.Equal(2, pos.Line);
        Assert.Equal(3, pos.Column);

        pos = lexer.Position(8);
        Assert.Equal(2, pos.Line);
        Assert.Equal(4, pos.Column);

        pos = lexer.Position(9);
        Assert.Equal(2, pos.Line);
        Assert.Equal(5, pos.Column);

        pos = lexer.Position(10);
        Assert.Equal(3, pos.Line);
        Assert.Equal(1, pos.Column);

        pos = lexer.Position(11);
        Assert.Equal(3, pos.Line);
        Assert.Equal(2, pos.Column);

        pos = lexer.Position(12);
        Assert.Equal(3, pos.Line);
        Assert.Equal(3, pos.Column);

        pos = lexer.Position(13);
        Assert.Equal(3, pos.Line);
        Assert.Equal(4, pos.Column);
    }
}
