using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Models;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker;

public class CSharpLiteralScannerTests
{
    [Theory]
    [InlineData("// Comment", 0)]
    [InlineData("x = 1; // Comment", 7)]
    [InlineData("x = 1;// Comment", 6)]
    [InlineData("var s = \"a\"; // Comment", 13)]
    [InlineData("var s = \"//\"; // Comment", 14)]
    [InlineData("var s = @\"//\"\"//\"; // Comment", 19)]
    [InlineData("var s = $\"{x}//\"; // Comment", 18)]
    [InlineData("var s = \"\"\"//\"\"\"; // Comment", 18)]
    [InlineData("var c = '/'; // Comment", 13)]
    [InlineData("var s = \"\\\"//\"; // Comment", 16)]
    [InlineData("x = a /* // */ + b; // Comment", 20)]
    public void FindLineComment_ReturnsStartOfComment(string text, int expected)
    {
        Assert.Equal(expected, CSharpLiteralScanner.FindLineComment(text));
    }

    [Theory]
    [InlineData("")]
    [InlineData("var x = a / b;")]
    [InlineData("var url = \"http://example.com\";")]
    [InlineData("var s = @\"C:\\\\temp//\";")]
    [InlineData("var c = '/';")]
    [InlineData("x = a /* // */ + b;")]
    [InlineData("x = a /* unclosed // block")]
    [InlineData("var s = \"unclosed // string")]
    public void FindLineComment_WhenNoComment_ReturnsMinusOne(string text)
    {
        Assert.Equal(-1, CSharpLiteralScanner.FindLineComment(text));
    }

    [Fact]
    public void FindComments_ReturnsLineAndBlockCommentsInOrder()
    {
        string content = "/* a */ x = 1; // b\r\n// c\n/* d\n e */";

        List<CommentSpanModel> comments = [.. CSharpLiteralScanner.FindComments(content)];

        Assert.Equal(
            ["/* a */", "// b", "// c", "/* d\n e */"],
            comments.Select(c => content[c.Start..c.End]));
        Assert.Equal([true, false, false, true], comments.Select(c => c.IsBlock));
    }

    [Fact]
    public void FindComments_FollowsMultiLineVerbatimAndRawStrings()
    {
        string content = "var a = @\"x\n// no\";\nvar b = \"\"\"\n/* no */\n\"\"\"; // yes";

        var comment = Assert.Single(CSharpLiteralScanner.FindComments(content));

        Assert.Equal("// yes", content[comment.Start..comment.End]);
    }

    [Fact]
    public void FindComments_WhenBlockCommentIsUnclosed_RunsToEnd()
    {
        string content = "x; /* open\n// still block";

        var comment = Assert.Single(CSharpLiteralScanner.FindComments(content));

        Assert.True(comment.IsBlock);
        Assert.Equal(content.Length, comment.End);
    }

    [Fact]
    public void FindComments_WhenNoComments_ReturnsEmpty()
    {
        Assert.Empty(CSharpLiteralScanner.FindComments("var url = \"http://example.com\"; var c = '/';"));
    }
}
