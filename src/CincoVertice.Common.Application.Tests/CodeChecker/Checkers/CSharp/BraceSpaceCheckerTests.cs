using CincoVertice.Common.Application.CodeStandard.Checkers.CSharp;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers.CSharp;

public class BraceSpaceCheckerTests
{
    private readonly BraceSpaceChecker _checker = new();

    [Theory]
    [InlineData("get{return (byte)((RgbColor >> 8) & 0xFF);}")]
    [InlineData("get {return x; }")]
    [InlineData("get { return x;}")]
    [InlineData("void Run() {}")]
    [InlineData("var a = new[] {1, 2 };")]
    public void Check_WhenBraceLacksSpace_AddsCh0033(string content)
    {
        var line = TestLine.Create("    " + content);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0033), error.Code);
        Assert.Equal(Errors.CH0033, error.Message);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("});")]
    [InlineData("get { return x; }")]
    [InlineData("void Run() { }")]
    [InlineData("var s = $\"{a}{b}\";")]
    [InlineData("var c = '{';")]
    [InlineData("var x = /* {a} */ 1;")]
    [InlineData(" * {a}")]
    [InlineData("Foo([{ A = 1 }]);")]
    public void Check_WhenBracesAreSpacedOrInLiteralsOrComments_ReturnsNoErrors(string content)
    {
        var line = TestLine.Create("    " + content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void Check_WhenLineStartsInsideLiteral_ReturnsNoErrors()
    {
        var line = TestLine.Create("{a}{b}\";");
        line.StartsInsideLiteral = true;

        _checker.Check(line);

        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData("    var x = 1; // {a}")]
    [InlineData("    /// <see cref=\"List{T}\"/>")]
    public void CheckLine_WhenBraceIsInComment_ReturnsNoCh0033(string content)
    {
        LineModel line = new() { Content = content, Number = 1 };

        new LineCheckerService().CheckLine(line);

        Assert.DoesNotContain(line.Errors, e => e.Code == nameof(Errors.CH0033));
    }
}
