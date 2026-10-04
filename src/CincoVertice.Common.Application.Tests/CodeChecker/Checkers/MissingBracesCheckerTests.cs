using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class MissingBracesCheckerTests
{
    private readonly MissingBracesChecker _checker = new();

    [Theory]
    [InlineData("if (x) { return; }")]
    [InlineData("if (x) {")]
    [InlineData("} else if (x) {")]
    [InlineData("} else {")]
    [InlineData("else { y = 1; }")]
    [InlineData("var elsewhere = 1;")]
    [InlineData("if (s == \")\") { return; }")]
    public void Check_WhenSingleLineIsBraced_ContinuesWithoutError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
        Assert.False(line.AwaitsOpeningBrace);
    }

    [Theory]
    [InlineData("if (x) return;")]
    [InlineData("    if (x) return;")]
    [InlineData("else if (x == 1) y = 2;")]
    [InlineData("} else if (Check(a, b)) y = 2;")]
    [InlineData("else y = 3;")]
    [InlineData("} else y = 3;")]
    [InlineData("if (s == \")\") return;")]
    public void Check_WhenBodyIsOnSameLine_AddsCh0031(string content)
    {
        var line = TestLine.Create(content, number: 5);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0031), error.Code);
        Assert.Equal(Errors.CH0031, error.Message);
        Assert.Equal(5, error.Line);
    }

    [Fact]
    public void Check_WhenBodyIsOnNextLine_AddsCh0031OnBodyLine()
    {
        var errors = CheckLines("if (x)", "return;");

        Assert.Equal([(2, nameof(Errors.CH0031))], errors);
    }

    [Fact]
    public void Check_WhenBareElseBodyIsOnNextLine_AddsCh0031OnBodyLine()
    {
        var errors = CheckLines("{", "}", "else", "y = 1;");

        Assert.Equal([(4, nameof(Errors.CH0031))], errors);
    }

    [Fact]
    public void Check_WhenBraceIsOnNextLine_ReturnsNoErrors()
    {
        var errors = CheckLines("if (x)", "{", "return;", "}", "else", "{", "y = 1;", "}");

        Assert.Empty(errors);
    }

    [Fact]
    public void Check_FollowsConditionSpanningLines()
    {
        var errors = CheckLines("if (a &&", "    (b || c))", "return;");

        Assert.Equal([(3, nameof(Errors.CH0031))], errors);
    }

    [Fact]
    public void Check_WhenConditionSpanningLinesIsBraced_ReturnsNoErrors()
    {
        var errors = CheckLines("if (a &&", "    b)", "{", "return;", "}");

        Assert.Empty(errors);
    }

    [Fact]
    public void Check_WhenConditionSpanningLinesHasBodyOnLastLine_AddsCh0031()
    {
        var errors = CheckLines("if (a &&", "    b) return;");

        Assert.Equal([(2, nameof(Errors.CH0031))], errors);
    }

    [Fact]
    public void Check_WhenNestedIfHasNoBraces_ReportsBothBodies()
    {
        var errors = CheckLines("if (a)", "if (b)", "return;");

        Assert.Equal([(2, nameof(Errors.CH0031)), (3, nameof(Errors.CH0031))], errors);
    }

    /// <summary>
    ///     Checks the lines in order, linking each to the previous one as CodeCheckerService does.
    /// </summary>
    private List<(int Line, string Code)> CheckLines(params string[] contents)
    {
        List<(int, string)> errors = [];
        LineModel? previous = null;

        for (int i = 0; i < contents.Length; i++)
        {
            var line = TestLine.Create(contents[i], number: i + 1);
            line.PreviousCodeLine = previous;

            _checker.Check(line);

            errors.AddRange(line.Errors.Select(e => (e.Line, e.Code)));
            previous = line;
        }

        return errors;
    }
}
