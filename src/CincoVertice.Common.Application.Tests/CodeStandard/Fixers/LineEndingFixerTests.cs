using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class LineEndingFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new LineEndingFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Fix_ConvertsCrlfToLf()
    {
        // Act
        var result = new LineEndingFixer().Fix("line1\r\nline2");

        // Assert
        Assert.Equal("line1\nline2", result);
    }

    [Fact]
    public void Fix_ConvertsCrOnlyToLf()
    {
        // Act
        var result = new LineEndingFixer().Fix("line1\rline2");

        // Assert
        Assert.Equal("line1\nline2", result);
    }

    [Fact]
    public void Fix_WhenAlreadyLf_LeavesUnchanged()
    {
        // Act
        var result = new LineEndingFixer().Fix("line1\nline2\n");

        // Assert
        Assert.Equal("line1\nline2\n", result);
    }

    [Fact]
    public void Fix_NormalizesMixedLineEndings()
    {
        // Act
        var result = new LineEndingFixer().Fix("line1\r\nline2\nline3\rline4");

        // Assert
        Assert.Equal("line1\nline2\nline3\nline4", result);
    }

    [Fact]
    public void Fix_WhenContentDoesNotEndWithNewLine_AppendsLf()
    {
        // Act
        var result = new LineEndingFixer().Fix("line1\nline2");

        // Assert
        Assert.Equal("line1\nline2", result);
    }

    [Fact]
    public void Fix_WhenContentAlreadyEndsWithLf_DoesNotAppendExtra()
    {
        // Act
        var result = new LineEndingFixer().Fix("line1\nline2\n");

        // Assert
        Assert.DoesNotContain("\n\n", result.TrimStart());
        Assert.EndsWith("\n", result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        string content = "line1\r\nline2\r\n";

        // Act
        var once = new LineEndingFixer().Fix(content);
        var twice = new LineEndingFixer().Fix(once);

        // Assert
        Assert.Equal(once, twice);
    }

    [Fact]
    public void Fix_MultipleConsecutiveCrlf_ConvertsEach()
    {
        // Act
        var result = new LineEndingFixer().Fix("a\r\n\r\nb");

        // Assert
        Assert.Equal("a\n\nb", result);
    }
}
