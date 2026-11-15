using CincoVertice.Common.Utils.Strings;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Strings;

public class DelimiterTest
{
    [Theory]
    [InlineData("A = B ", "=", false, " B ")]
    [InlineData("A = B ", "=", true, "B")]
    [InlineData("A = B ", " ", false, "= B ")]
    [InlineData("A = B ", " ", true, "= B")]
    [InlineData("A = B ", "?", false, "")]
    [InlineData("A = B ", "?", true, "")]
    [InlineData("A = B ", "= B", false, " ")]
    [InlineData("A = B ", "= B", true, "")]
    public void DelitimerAfter_ReturnsExpectedResult(
        string testStr,
        string delimiter,
        bool trim,
        string expectedResult)
    {
        // Act
        var result = Delimiter.After(testStr, delimiter, trim);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("\r\n", "=", false, " B ")]
    [InlineData("\n", "=", true, "B")]
    [InlineData("\r\n", "\r\n\r\n", false, " = B ")]
    [InlineData("\r\n", "\r\n\r\n", true, "= B")]
    [InlineData("\n", "\n\n", false, " = B ")]
    [InlineData("\n", "\n\n", true, "= B")]
    public void DelitimerAfter_TestMultilineString_ReturnsExpectedResult(
        string newLine,
        string delimiter,
        bool trim,
        string expectedResult)
    {
        // Arrange
        var testStr = "A" + newLine + newLine + " = B ";

        // Act
        var result = Delimiter.After(testStr, delimiter, trim);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}
