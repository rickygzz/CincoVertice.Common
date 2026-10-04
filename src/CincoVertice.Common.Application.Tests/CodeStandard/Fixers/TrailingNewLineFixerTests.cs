using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class TrailingNewLineFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new TrailingNewLineFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Fix_WhenNoLineEndingAndNoTrailingNewline_AppendsLf()
    {
        // Act
        var result = new TrailingNewLineFixer().Fix("abc");

        // Assert
        Assert.Equal("abc\n", result);
    }

    // --- LF line endings ---

    [Fact]
    public void Fix_WhenLfFileAlreadyEndsWithLf_LeavesUnchanged()
    {
        // Arrange
        string content = "line1\nline2\n";

        // Act
        var result = new TrailingNewLineFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_WhenLfFileDoesNotEndWithNewLine_AppendsLf()
    {
        // Act
        var result = new TrailingNewLineFixer().Fix("line1\nline2");

        // Assert
        Assert.Equal("line1\nline2\n", result);
    }

    [Fact]
    public void Fix_WhenLfFileHasMultipleTrailingNewLines_ReducesToOne()
    {
        // Act
        var result = new TrailingNewLineFixer().Fix("line1\nline2\n\n\n");

        // Assert
        Assert.Equal("line1\nline2\n", result);
    }

    [Fact]
    public void Fix_WhenCrlfFileAlreadyEndsWithCrlf_LeavesUnchanged()
    {
        // Arrange
        string content = "line1\r\nline2\r\n";

        // Act
        var result = new TrailingNewLineFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_WhenCrlfFileDoesNotEndWithNewLine_AppendsCrlf()
    {
        // Act
        var result = new TrailingNewLineFixer().Fix("line1\r\nline2");

        // Assert
        Assert.Equal("line1\r\nline2\r\n", result);
    }

    [Fact]
    public void Fix_WhenCrlfFileHasMultipleTrailingNewLines_ReducesToOne()
    {
        // Act
        var result = new TrailingNewLineFixer().Fix("line1\r\nline2\r\n\r\n");

        // Assert
        Assert.Equal("line1\r\nline2\r\n", result);
    }

    // --- CR-only line endings (legacy Mac) ---

    [Fact]
    public void Fix_WhenCrFileAlreadyEndsWithCr_LeavesUnchanged()
    {
        // Arrange
        string content = "line1\rline2\r";

        // Act
        var result = new TrailingNewLineFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_WhenCrFileDoesNotEndWithNewLine_AppendsCr()
    {
        // Act
        var result = new TrailingNewLineFixer().Fix("line1\rline2");

        // Assert
        Assert.Equal("line1\rline2\r", result);
    }

    // --- Detection uses first line ending ---

    [Fact]
    public void Fix_WhenFirstLineEndingIsLfButEndsWithCrlf_UsesLf()
    {
        // First \n is detected; trailing \r\n is stripped and replaced with \n.
        var result = new TrailingNewLineFixer().Fix("line1\nline2\r\n");

        Assert.Equal("line1\nline2\n", result);
    }

    [Theory]
    [InlineData("line1\nline2")]
    [InlineData("line1\r\nline2")]
    [InlineData("line1\nline2\n\n")]
    public void Fix_IsIdempotent(string content)
    {
        // Act
        var once = new TrailingNewLineFixer().Fix(content);
        var twice = new TrailingNewLineFixer().Fix(once);

        // Assert
        Assert.Equal(once, twice);
    }
}
