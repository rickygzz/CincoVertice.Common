using CincoVertice.Common.Application.CodeStandard.Fixers.CSharp;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers.CSharp;

public class ForbiddenStartTokenFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new ForbiddenStartTokenFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Fix_MovesClosingParenthesisAndSemicolonUpAndRemovesEmptyLine()
    {
        // Arrange
        string content =
            "Foo(\n"
            + "    a\n"
            + ");\n"
            + "Bar();\n";

        // Act
        var result = new ForbiddenStartTokenFixer().Fix(content);

        // Assert
        Assert.Equal("Foo(\n    a);\nBar();\n", result);
    }

    [Fact]
    public void Fix_MovesLeadingCommasToPreviousLine()
    {
        // Arrange
        string content =
            "Foo(a\n"
            + "    , b\n"
            + "    , c);\n";

        // Act
        var result = new ForbiddenStartTokenFixer().Fix(content);

        // Assert
        Assert.Equal("Foo(a,\n    b,\n    c);\n", result);
    }

    [Theory]
    [InlineData("var x\n    = Foo();\n", "var x =\n    Foo();\n")]
    [InlineData("public int X\n    => 5;\n", "public int X =>\n    5;\n")]
    [InlineData("bool b = a\n    == c;\n", "bool b = a ==\n    c;\n")]
    [InlineData("int s = a\n    >> 2;\n", "int s = a >>\n    2;\n")]
    [InlineData("x\n    += 1;\n", "x +=\n    1;\n")]
    public void Fix_MovesLeadingOperatorToPreviousLineWithSpace(string content, string expected)
    {
        // Act
        var result = new ForbiddenStartTokenFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Fix_PutsTokenBeforeTrailingCommentOfPreviousLine()
    {
        // Arrange
        string content =
            "Foo(a // first\n"
            + "    , b);\n";

        // Act
        var result = new ForbiddenStartTokenFixer().Fix(content);

        // Assert
        Assert.Equal("Foo(a, // first\n    b);\n", result);
    }

    [Fact]
    public void Fix_SkipsCommentAndBlankLinesToFindPreviousCodeLine()
    {
        // Arrange
        string content =
            "var x\n"
            + "    // why\n"
            + "\n"
            + "    = 1;\n";

        // Act
        var result = new ForbiddenStartTokenFixer().Fix(content);

        // Assert
        Assert.Equal("var x =\n    // why\n\n    1;\n", result);
    }

    [Theory]
    [InlineData("Foo();\n(a as IDisposable).Dispose();\n")]
    [InlineData("Foo();\n;\n")]
    [InlineData("{\n    , a\n")]
    [InlineData("#if DEBUG\n    , a\n#endif\n")]
    [InlineData(", a\n")]
    [InlineData("/*\n, a\n*/\n")]
    [InlineData("var s = @\"a\n, b\";\n")]
    public void Fix_LeavesLinesThatDoNotContinuePreviousLineUnchanged(string content)
    {
        // Act
        var result = new ForbiddenStartTokenFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_PreservesCrlfLineEndings()
    {
        // Act
        var result = new ForbiddenStartTokenFixer().Fix("Foo(\r\n    a\r\n);\r\n");

        // Assert
        Assert.Equal("Foo(\r\n    a);\r\n", result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        var fixer = new ForbiddenStartTokenFixer();
        string once = fixer.Fix("Foo(a\n    , b\n);\nvar x\n    = 1;\n");

        // Act
        string twice = fixer.Fix(once);

        // Assert
        Assert.Equal(once, twice);
    }
}
