using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class TrailingWhitespaceFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new TrailingWhitespaceFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Fix_RemovesTrailingSpacesFromLine()
    {
        // Act
        var result = new TrailingWhitespaceFixer().Fix("var x = 1;   \n");

        // Assert
        Assert.Equal("var x = 1;\n", result);
    }

    [Fact]
    public void Fix_RemovesTrailingTabsFromLine()
    {
        // Act
        var result = new TrailingWhitespaceFixer().Fix("var x = 1;\t\t\n");

        // Assert
        Assert.Equal("var x = 1;\n", result);
    }

    [Fact]
    public void Fix_RemovesMixedTrailingWhitespace()
    {
        // Act
        var result = new TrailingWhitespaceFixer().Fix("var x = 1; \t \n");

        // Assert
        Assert.Equal("var x = 1;\n", result);
    }

    [Fact]
    public void Fix_CleansEveryLineInContent()
    {
        // Act
        var result = new TrailingWhitespaceFixer().Fix("line1   \nline2  \nline3\n");

        // Assert
        Assert.Equal("line1\nline2\nline3\n", result);
    }

    [Fact]
    public void Fix_RemovesTrailingWhitespaceFromLastLine()
    {
        // Act
        var result = new TrailingWhitespaceFixer().Fix("var x = 1;   ");

        // Assert
        Assert.Equal("var x = 1;", result);
    }

    [Fact]
    public void Fix_WhenNoTrailingWhitespace_LeavesUnchanged()
    {
        // Arrange
        string content = "line1\nline2\n";

        // Act
        var result = new TrailingWhitespaceFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_PreservesLeadingWhitespace()
    {
        // Act
        var result = new TrailingWhitespaceFixer().Fix("    var x = 1;   \n");

        // Assert
        Assert.Equal("    var x = 1;\n", result);
    }

    [Theory]
    [InlineData("line1   \r\nline2   \r\n", "line1\r\nline2\r\n")]
    [InlineData("line1   \nline2   \n", "line1\nline2\n")]
    [InlineData("     \n", "\n")]
    [InlineData("     \n      \n", "\n\n")]
    public void Fix_PreservesLineEndings(string content, string expected)
    {
        // Act
        var result = new TrailingWhitespaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        string content = "line1   \nline2\t\n";

        // Act
        var once = new TrailingWhitespaceFixer().Fix(content);
        var twice = new TrailingWhitespaceFixer().Fix(once);

        // Assert
        Assert.Equal(once, twice);
    }

    [Fact]
    public void Fix_DoesNotRemoveInternalWhitespace()
    {
        // Act
        var result = new TrailingWhitespaceFixer().Fix("var x  =  1;  \n");

        // Assert
        Assert.Equal("var x  =  1;\n", result);
    }
}
