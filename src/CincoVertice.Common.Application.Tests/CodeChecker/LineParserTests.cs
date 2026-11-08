using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Models;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker;

public class LineParserTests
{
    [Fact]
    public void GetLines_WhenContentIsEmpty_ReturnsNoLines()
    {
        // Act
        var lines = LineParser.GetLines(string.Empty);

        // Assert
        Assert.Empty(lines);
    }

    [Fact]
    public void GetLines_WhenSingleLineHasNoEnding_ReturnsUndefinedEnding()
    {
        // Act
        var lines = LineParser.GetLines("public class Test");

        // Assert
        var line = Assert.Single(lines);

        Assert.Equal(1, line.Number);
        Assert.Equal("public class Test", line.Content);
        Assert.Equal(LineEndingEnum.None, line.LineEnding);
    }

    [Theory]
    [InlineData("a\r\nb", "a", LineEndingEnum.CRLF)]
    [InlineData("a\nb", "a", LineEndingEnum.LF)]
    [InlineData("a\rb", "a", LineEndingEnum.CR)]
    [InlineData("\r\nb", "", LineEndingEnum.CRLF)]
    [InlineData("\nb", "", LineEndingEnum.LF)]
    [InlineData("\rb", "", LineEndingEnum.CR)]
    public void GetLines_DetectsLineEndingOfTerminatedLine(string content, string line1Content, LineEndingEnum expected)
    {
        // Act
        var lines = LineParser.GetLines(content);

        // Assert
        Assert.Equal(2, lines.Count);

        Assert.Equal(1, lines[0].Number);
        Assert.Equal(line1Content, lines[0].Content);
        Assert.Equal(expected, lines[0].LineEnding);

        Assert.Equal(2, lines[1].Number);
        Assert.Equal("b", lines[1].Content);
        Assert.Equal(LineEndingEnum.None, lines[1].LineEnding);
    }

    [Fact]
    public void GetLines_WhenEndingsAreMixed_DetectsEachLineIndependently()
    {
        // Act
        var lines = LineParser.GetLines("a\r\nb\nc\rd");

        // Assert
        Assert.Equal(4, lines.Count);

        Assert.Equal(LineEndingEnum.CRLF, lines[0].LineEnding);
        Assert.Equal(1, lines[0].Number);

        Assert.Equal(LineEndingEnum.LF, lines[1].LineEnding);
        Assert.Equal(2, lines[1].Number);

        Assert.Equal(LineEndingEnum.CR, lines[2].LineEnding);
        Assert.Equal(3, lines[2].Number);

        Assert.Equal(LineEndingEnum.None, lines[3].LineEnding);
        Assert.Equal(4, lines[3].Number);
    }

    [Fact]
    public void GetLines_WhenContentEndsWithNewline_DoesNotAddTrailingEmptyLine()
    {
        // Act
        var lines = LineParser.GetLines("a\n");

        // Assert
        var line = Assert.Single(lines);

        Assert.Equal(1, line.Number);
        Assert.Equal("a", line.Content);
        Assert.Equal(LineEndingEnum.LF, line.LineEnding);
    }

    [Fact]
    public void GetLines_WhenConsecutiveNewlines_ReturnsEmptyLineBetween()
    {
        // Act
        var lines = LineParser.GetLines("a\n\nb");

        // Assert
        Assert.Equal(3, lines.Count);

        Assert.Equal(1, lines[0].Number);
        Assert.Equal("a", lines[0].Content);
        Assert.Equal(LineEndingEnum.LF, lines[0].LineEnding);

        Assert.Equal(2, lines[1].Number);
        Assert.Equal(string.Empty, lines[1].Content);
        Assert.Equal(LineEndingEnum.LF, lines[1].LineEnding);

        Assert.Equal(3, lines[2].Number);
        Assert.Equal("b", lines[2].Content);
        Assert.Equal(LineEndingEnum.None, lines[2].LineEnding);
    }

    [Fact]
    public void GetLines_AssignsSequentialLineNumbersStartingAtOne()
    {
        // Act
        var lines = LineParser.GetLines("a\nb\nc");

        // Assert
        Assert.Equal(3, lines.Count);
        Assert.Equal(1, lines[0].Number);
        Assert.Equal(2, lines[1].Number);
        Assert.Equal(3, lines[2].Number);
    }

    [Fact]
    public void GetLines_SetsContentAndTrimmedContent()
    {
        // Act
        var lines = LineParser.GetLines("    indented code    \n");

        // Assert
        var line = Assert.Single(lines);
        Assert.Equal("    indented code    ", line.Content);
        Assert.Equal("indented code", line.TrimmedContent);
    }

    [Fact]
    public void GetLines_WhenLoneCarriageReturnAtEnd_DetectsCr()
    {
        // Act
        var lines = LineParser.GetLines("a\r");

        // Assert
        var line = Assert.Single(lines);
        Assert.Equal("a", line.Content);
        Assert.Equal(LineEndingEnum.CR, line.LineEnding);
    }
}
