using CincoVertice.Common.Application.CodeStandard.Helper;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker;

public class JavaScriptCodeScannerTests
{
    private readonly JavaScriptCodeScanner _scanner = JavaScriptCodeScanner.Instance;

    private List<string> Literals(string code) =>
        [.. _scanner.FindLiterals(code).Select(literal => code[literal.Start..literal.End])];

    private List<string> Comments(string code) =>
        [.. _scanner.FindComments(code).Select(comment => code[comment.Start..comment.End])];

    [Theory]
    [InlineData("x = 'a' + \"b\";", "'a'|\"b\"")]
    [InlineData("x = 'it\\'s';", "'it\\'s'")]
    [InlineData("x = \"say \\\"hi\\\"\";", "\"say \\\"hi\\\"\"")]
    [InlineData("x = `a ${b} c`;", "`a ${b} c`")]
    [InlineData("x = `${a + `in ${b}`} }`;", "`${a + `in ${b}`} }`")]
    [InlineData("x = `${ {a: '}'}.a }`;", "`${ {a: '}'}.a }`")]
    public void FindLiterals_FindsStringsAndTemplates(string code, string expected)
    {
        Assert.Equal(expected.Split('|'), Literals(code));
    }

    [Fact]
    public void FindLiterals_FollowsATemplateAcrossLines()
    {
        // Arrange
        string code = "const q = `\n  select *\n  // not a comment\n`;\nx();";

        // Act & Assert
        Assert.Equal(["`\n  select *\n  // not a comment\n`"], Literals(code));
        Assert.Empty(Comments(code));
    }

    [Theory]
    [InlineData("s.replace(/\\/+$/, '')", "/\\/+$/|''")]
    [InlineData("const re = /a[/]b/gi;", "/a[/]b/gi")]
    [InlineData("if (/^x/.test(s)) {}", "/^x/")]
    [InlineData("return /x/.test(s);", "/x/")]
    [InlineData("f(a, /,/)", "/,/")]
    [InlineData("x = !/a/.test(s)", "/a/")]
    public void FindLiterals_FindsRegularExpressionsWhereAValueGoes(string code, string expected)
    {
        Assert.Equal(expected.Split('|'), Literals(code));
    }

    [Theory]
    [InlineData("const half = total / 2 / count;")]
    [InlineData("x = (a) / (b) / c;")]
    [InlineData("x = arr[0] / arr[1] / 2;")]
    [InlineData("x = 'a'.length / 2 / n;")]
    public void FindLiterals_ASlashAfterAValue_Divides(string code)
    {
        Assert.Equal(code.Contains('\'') ? ["'a'"] : [], Literals(code));
    }

    [Fact]
    public void FindLiterals_ARegularExpressionNotClosedOnItsLine_Divides()
    {
        // Arrange: the / after = would start one, but nothing closes it
        string code = "x = / 2;\ny = 'a';";

        // Act & Assert
        Assert.Equal(["'a'"], Literals(code));
    }

    [Theory]
    [InlineData("x(); // it's", "// it's")]
    [InlineData("url = 'http://x'; // real", "// real")]
    [InlineData("re = /\\/\\//; /* real */", "/* real */")]
    [InlineData("t = `//not`; //yes", "//yes")]
    public void FindComments_IgnoresDelimitersInsideLiterals(string code, string expected)
    {
        Assert.Equal([expected], Comments(code));
    }

    [Fact]
    public void FindComments_QuotesInsideCommentsDontStartLiterals()
    {
        // Arrange
        string code = "/* don't */ x = 'a'; // \"b";

        // Act & Assert
        Assert.Equal(["'a'"], Literals(code));
        Assert.Equal(["/* don't */", "// \"b"], Comments(code));
    }
}
