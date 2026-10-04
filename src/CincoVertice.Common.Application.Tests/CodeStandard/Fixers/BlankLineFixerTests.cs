using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class BlankLineFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new BlankLineFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    // --- Single blank line is preserved when not before } ---

    [Fact]
    public void Fix_WhenNoBlankLines_LeavesUnchanged()
    {
        // Arrange
        string content = "line1\nline2\nline3\n";

        // Act
        var result = new BlankLineFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_WhenSingleBlankLineBetweenCode_LeavesUnchanged()
    {
        // Arrange
        string content = "somecode;\n\nmorecode;";

        // Act
        var result = new BlankLineFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_WhenSingleBlankLineCrlf_LeavesUnchanged()
    {
        // Arrange
        string content = "somecode;\r\n\r\nmorecode;";

        // Act
        var result = new BlankLineFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    // --- Two or more blank lines are reduced to one ---

    [Fact]
    public void Fix_ReducesTwoBlankLinesToOne()
    {
        // Act
        var result = new BlankLineFixer().Fix("line1\n\n\nline2");

        // Assert
        Assert.Equal("line1\n\nline2", result);
    }

    [Fact]
    public void Fix_ReducesThreeBlankLinesToOne()
    {
        // Act
        var result = new BlankLineFixer().Fix("line1\n\n\n\nline2");

        // Assert
        Assert.Equal("line1\n\nline2", result);
    }

    [Fact]
    public void Fix_ReducesCrlfTwoBlankLinesToOne()
    {
        // Act
        var result = new BlankLineFixer().Fix("line1\r\n\r\n\r\nline2");

        // Assert
        Assert.Equal("line1\r\n\r\nline2", result);
    }

    [Fact]
    public void Fix_ReducesMultipleGroupsIndependently()
    {
        // Act
        var result = new BlankLineFixer().Fix("a\n\nb\n\n\nc\n\n\n\nd");

        // Assert
        Assert.Equal("a\n\nb\n\nc\n\nd", result);
    }

    // --- Blank lines before } are removed ---

    [Fact]
    public void Fix_RemovesSingleBlankLineBeforeClosingBrace()
    {
        // Arrange
        string content =
            "    }\n" +
            "\n" +
            "}";

        // Act
        var result = new BlankLineFixer().Fix(content);

        // Assert
        Assert.Equal("    }\n}", result);
    }

    [Fact]
    public void Fix_RemovesMultipleBlankLinesBeforeClosingBrace()
    {
        // Arrange
        string content =
            "    }\n" +
            "\n" +
            "\n" +
            "}";

        // Act
        var result = new BlankLineFixer().Fix(content);

        // Assert
        Assert.Equal("    }\n}", result);
    }

    [Fact]
    public void Fix_RemovesBlankLineBeforeIndentedClosingBrace()
    {
        // Arrange
        string content =
            "    code;\n" +
            "\n" +
            "    }";

        // Act
        var result = new BlankLineFixer().Fix(content);

        // Assert
        Assert.Equal("    code;\n    }", result);
    }

    [Fact]
    public void Fix_RemovesBlankLinesBeforeClosingBraceButPreservesOthers()
    {
        // Arrange
        string content =
            "class Foo\n" +
            "{\n" +
            "    void A() { }\n" +
            "\n" +
            "    void B() { }\n" +
            "\n" +
            "}";

        // Act
        var result = new BlankLineFixer().Fix(content);

        // Assert
        Assert.Equal(
            "class Foo\n" +
            "{\n" +
            "    void A() { }\n" +
            "\n" +
            "    void B() { }\n" +
            "}",
            result);
    }

    // --- Content is untouched ---

    [Fact]
    public void Fix_PreservesContentBetweenLines()
    {
        // Arrange
        string content = "class Foo\n{\n    int x;\n}";

        // Act
        var result = new BlankLineFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    // --- Idempotency ---

    [Theory]
    [InlineData("line1\n\nline2")]
    [InlineData("line1\n\n\nline2")]
    [InlineData("    }\n\n}")]
    [InlineData("line1\r\n\r\n\r\nline2")]
    public void Fix_IsIdempotent(string content)
    {
        // Act
        var once = new BlankLineFixer().Fix(content);
        var twice = new BlankLineFixer().Fix(once);

        // Assert
        Assert.Equal(once, twice);
    }
}
