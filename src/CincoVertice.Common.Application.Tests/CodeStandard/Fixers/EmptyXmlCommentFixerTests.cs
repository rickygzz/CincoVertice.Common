using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class EmptyXmlCommentFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new EmptyXmlCommentFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Fix_RemovesEmptyMultiLineSummaryBlock()
    {
        // Arrange
        string content =
            "/// <summary>\n"
            + "/// \n"
            + "/// </summary>\n"
            + "public void Foo() { }";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal("public void Foo() { }", result);
    }

    [Fact]
    public void Fix_RemovesInlineSummaryBlockWithNoText()
    {
        // Arrange
        string content =
            "/// <summary></summary>\n"
            + "public void Foo() { }";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal("public void Foo() { }", result);
    }

    [Fact]
    public void Fix_RemovesBlockWithOnlyBlankDocLines()
    {
        // Arrange
        string content =
            "///\n"
            + "///\n"
            + "public void Foo() { }";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal("public void Foo() { }", result);
    }

    // --- Block with content is preserved ---

    [Fact]
    public void Fix_WhenSummaryHasText_LeavesBlockUnchanged()
    {
        // Arrange
        string content =
            "/// <summary>\n"
            + "///     Does something useful.\n"
            + "/// </summary>\n"
            + "public void Foo() { }";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    // --- Inline empty tag removal within a meaningful block ---

    [Fact]
    public void Fix_RemovesEmptyParamTagFromBlockWithContent()
    {
        // Arrange
        string content =
            "/// <summary>\n"
            + "///     Does something.\n"
            + "/// </summary>\n"
            + "/// <param name=\"x\"></param>\n"
            + "public void Foo(int x) { }";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal(
            "/// <summary>\n"
            + "///     Does something.\n"
            + "/// </summary>\n"
            + "public void Foo(int x) { }",
            result);
    }

    [Fact]
    public void Fix_RemovesEmptyReturnsTagFromBlockWithContent()
    {
        // Arrange
        string content =
            "/// <summary>\n"
            + "///     Gets the value.\n"
            + "/// </summary>\n"
            + "/// <returns></returns>\n"
            + "public int GetValue() => 0;";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal(
            "/// <summary>\n"
            + "///     Gets the value.\n"
            + "/// </summary>\n"
            + "public int GetValue() => 0;",
            result);
    }

    [Fact]
    public void Fix_RemovesMultipleEmptyInlineTags()
    {
        // Arrange
        string content =
            "/// <summary>\n"
            + "///     Processes the request.\n"
            + "/// </summary>\n"
            + "/// <param name=\"a\"></param>\n"
            + "/// <param name=\"b\"></param>\n"
            + "/// <returns></returns>\n"
            + "public int Process(int a, int b) => 0;";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal(
            "/// <summary>\n"
            + "///     Processes the request.\n"
            + "/// </summary>\n"
            + "public int Process(int a, int b) => 0;",
            result);
    }

    // --- Non-doc lines are untouched ---

    [Fact]
    public void Fix_DoesNotAffectRegularCode()
    {
        // Arrange
        string content = "var x = 1;\nreturn x;";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_DoesNotAffectRegularComments()
    {
        // Arrange
        string content = "// This is a regular comment\nvar x = 1;";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    // --- CRLF line endings ---

    [Fact]
    public void Fix_RemovesEmptyBlockWithCrlfLineEndings()
    {
        // Arrange
        string content =
            "/// <summary>\r\n"
            + "/// \r\n"
            + "/// </summary>\r\n"
            + "public void Foo() { }";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal("public void Foo() { }", result);
    }

    // --- Idempotency ---

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        string content =
            "/// <summary>\n"
            + "///     Does something.\n"
            + "/// </summary>\n"
            + "/// <param name=\"x\"></param>\n"
            + "public void Foo(int x) { }";

        // Act
        var once = new EmptyXmlCommentFixer().Fix(content);
        var twice = new EmptyXmlCommentFixer().Fix(once);

        // Assert
        Assert.Equal(once, twice);
    }

    [Fact]
    public void Fix_KeepsInheritDocBlock()
    {
        // Arrange
        string content =
            "/// <inheritdoc/>\n"
            + "public void Foo() { }";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Theory]
    [InlineData("/// <returns></returns> The value.", "/// The value.")]
    [InlineData("///     Uses <c></c> nothing.", "///     Uses nothing.")]
    [InlineData("/// Text <remarks></remarks>", "/// Text")]
    public void Fix_RemovesEmptyTagFromLineWithText(string line, string expected)
    {
        // Act
        var result = new EmptyXmlCommentFixer().Fix(line + "\npublic void Foo() { }");

        // Assert
        Assert.Equal(expected + "\npublic void Foo() { }", result);
    }

    [Fact]
    public void Fix_RemovesEmptyTypeParamAndRemarks()
    {
        // Arrange
        string content =
            "/// <summary>Text.</summary>\n"
            + "/// <typeparam name=\"T\"></typeparam>\n"
            + "/// <remarks></remarks>\n"
            + "public void Foo<T>() { }";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal("/// <summary>Text.</summary>\npublic void Foo<T>() { }", result);
    }

    [Fact]
    public void Fix_LeavesSeparatorLinesUnchanged()
    {
        // Arrange
        string content = "////////////\n//// <summary></summary>\nvar x = 1;";

        // Act
        var result = new EmptyXmlCommentFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }
}
