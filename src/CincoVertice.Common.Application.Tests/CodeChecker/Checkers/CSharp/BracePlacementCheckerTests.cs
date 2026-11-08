using CincoVertice.Common.Application.CodeStandard.Checkers.CSharp;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers.CSharp;

public class BracePlacementCheckerTests
{
    private readonly BracePlacementChecker _checker = new();

    [Theory]
    [InlineData("if (Arrays.Equals(ace.certVerifier, certVerifier)) {")]
    [InlineData("} else {")]
    [InlineData("} else")]
    [InlineData("} catch (Exception ex)")]
    [InlineData("public void Run() {")]
    [InlineData("list.ForEach(x => {")]
    public void Check_WhenBlockBraceSharesLine_AddsCh0034(string content)
    {
        var line = TestLine.Create("    " + content);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0034), error.Code);
        Assert.Equal(Errors.CH0034, error.Message);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("});")]
    [InlineData("},")]
    [InlineData("} while (a > b);")]
    [InlineData("get { return x; }")]
    [InlineData("var s = @\"{")]
    [InlineData("var s = \"{\";")]
    [InlineData(" * } else {")]
    public void Check_WhenBracesAreOnOwnLineOrNotBlockBraces_ReturnsNoErrors(string content)
    {
        var line = TestLine.Create("    " + content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void Check_WhenLineStartsInsideLiteral_ReturnsNoErrors()
    {
        var line = TestLine.Create("} else {");
        line.StartsInsideLiteral = true;

        _checker.Check(line);

        Assert.Empty(line.Errors);
    }

    [Fact]
    public void CheckLine_WhenBraceIsInTrailingComment_ReturnsNoCh0034()
    {
        LineModel line = new() { Content = "    var x = 1; // {", Number = 1 };

        new LineCheckerService().CheckLine(line);

        Assert.DoesNotContain(line.Errors, e => e.Code == nameof(Errors.CH0034));
    }
}
