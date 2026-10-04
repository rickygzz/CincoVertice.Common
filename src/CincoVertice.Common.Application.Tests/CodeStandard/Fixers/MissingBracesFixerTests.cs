using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class MissingBracesFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new MissingBracesFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Fix_AddsbracesWhenBodyIsOnNextLine()
    {
        // Arrange
        string content =
            "if (x)\n"
            + "    DoSomething();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (x)\n"
            + "{\n"
            + "    DoSomething();\n"
            + "}\n",
            result);
    }

    [Fact]
    public void Fix_AddsBracesWhenBodyIsOnSameLine()
    {
        // Arrange
        string content = "if (x) DoSomething();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (x)\n"
            + "{\n"
            + "    DoSomething();\n"
            + "}\n",
            result);
    }

    [Fact]
    public void Fix_PreservesIndentationOfIfBlock()
    {
        // Arrange
        string content =
            "    if (x)\n"
            + "        DoSomething();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "    if (x)\n"
            + "    {\n"
            + "        DoSomething();\n"
            + "    }\n",
            result);
    }

    [Fact]
    public void Fix_WhenIfAlreadyHasBraces_LeavesUnchanged()
    {
        // Arrange
        string content =
            "if (x)\n"
            + "{\n"
            + "    DoSomething();\n"
            + "}\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_HandlesMultilineCondition()
    {
        // Arrange
        string content =
            "if (a\n"
            + "    && b)\n"
            + "    DoSomething();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (a\n"
            + "    && b)\n"
            + "{\n"
            + "    DoSomething();\n"
            + "}\n",
            result);
    }

    [Fact]
    public void Fix_HandlesConditionWithStringContainingParens()
    {
        // Arrange
        string content =
            "if (x.Contains(\"(\"))\n"
            + "    DoSomething();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (x.Contains(\"(\"))\n"
            + "{\n"
            + "    DoSomething();\n"
            + "}\n",
            result);
    }

    [Fact]
    public void Fix_AddsBracesToElseIfWithoutBraces()
    {
        // Arrange
        string content =
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else if (y)\n"
            + "    B();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else if (y)\n"
            + "{\n"
            + "    B();\n"
            + "}\n",
            result);
    }

    [Fact]
    public void Fix_AddsBracesToBareElseWithBodyOnNextLine()
    {
        // Arrange
        string content =
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else\n"
            + "    B();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else\n"
            + "{\n"
            + "    B();\n"
            + "}\n",
            result);
    }

    [Fact]
    public void Fix_AddsBracesToBareElseWithBodyOnSameLine()
    {
        // Arrange
        string content =
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else B();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else\n"
            + "{\n"
            + "    B();\n"
            + "}\n",
            result);
    }

    [Fact]
    public void Fix_WhenElseHasInlineComment_TreatsBodyAsNextLine()
    {
        // Arrange - "else // comment" should not treat the comment as the body
        string content =
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else // retryable — clear and re-submit\n"
            + "{\n"
            + "    B();\n"
            + "}\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_WhenElseHasInlineCommentAndNoBraces_AddsBraces()
    {
        // Arrange
        string content =
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else // permanent\n"
            + "    B();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else // permanent\n"
            + "{\n"
            + "    B();\n"
            + "}\n",
            result);
    }

    [Fact]
    public void Fix_WhenIfHasInlineComment_TreatsBodyAsNextLine()
    {
        // Arrange - "if (x) // comment" should not treat the comment as the body
        string content =
            "if (x) // check condition\n"
            + "{\n"
            + "    DoSomething();\n"
            + "}\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_WhenElseAlreadyHasBraces_LeavesUnchanged()
    {
        // Arrange
        string content =
            "if (x)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else\n"
            + "{\n"
            + "    B();\n"
            + "}\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_FixesEntireIfElseChain()
    {
        // Arrange
        string content =
            "if (a)\n"
            + "    A();\n"
            + "else if (b)\n"
            + "    B();\n"
            + "else\n"
            + "    C();\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (a)\n"
            + "{\n"
            + "    A();\n"
            + "}\n"
            + "else if (b)\n"
            + "{\n"
            + "    B();\n"
            + "}\n"
            + "else\n"
            + "{\n"
            + "    C();\n"
            + "}\n",
            result);
    }

    [Fact]
    public void Fix_PreservesCrlfLineEndings()
    {
        // Arrange
        string content =
            "if (x)\r\n"
            + "    DoSomething();\r\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "if (x)\r\n"
            + "{\r\n"
            + "    DoSomething();\r\n"
            + "}\r\n",
            result);
    }

    [Fact]
    public void Fix_AddsBracesWhenBodySpansMultipleLines()
    {
        // Arrange - body is a return statement with a multi-line anonymous object
        string content =
            "            if (tracking.State == PolicyState.Completed)\n"
            + "                return Conflict(new\n"
            + "                {\n"
            + "                    error = \"Policy is already in a completed state and cannot be reprocessed.\",\n"
            + "                    serialNo,\n"
            + "                    state = tracking.State\n"
            + "                });\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "            if (tracking.State == PolicyState.Completed)\n"
            + "            {\n"
            + "                return Conflict(new\n"
            + "                {\n"
            + "                    error = \"Policy is already in a completed state and cannot be reprocessed.\",\n"
            + "                    serialNo,\n"
            + "                    state = tracking.State\n"
            + "                });\n"
            + "            }\n",
            result);
    }

    [Fact]
    public void Fix_AddsBracesWhenBodySpansMultipleLinesV2()
    {
        // Arrange - body is a return statement with a multi-line anonymous object
        string content =
            "    if (statusCode is 401 or 403)\n"
            + "        throw new InvalidArgumentException(\n"
            + "            \"TokenAuthError\",\n"
            + "            $\"99Pay token request rejected with HTTP {statusCode}: {responseBody}\");\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(
            "    if (statusCode is 401 or 403)\n"
            + "    {\n"
            + "        throw new InvalidArgumentException(\n"
            + "            \"TokenAuthError\",\n"
            + "            $\"99Pay token request rejected with HTTP {statusCode}: {responseBody}\");\n"
            + "    }\n",
            result);
    }

    [Fact]
    public void Fix_DoesNotAffectCodeWithoutIf()
    {
        // Arrange
        string content = "var x = 1;\nreturn x;\n";

        // Act
        var result = new MissingBracesFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        string content =
            "if (x)\n"
            + "    DoSomething();\n";

        // Act
        var once = new MissingBracesFixer().Fix(content);
        var twice = new MissingBracesFixer().Fix(once);

        // Assert
        Assert.Equal(once, twice);
    }
}
