using CincoVertice.Common.Application.CodeStandard.Checkers.CSharp;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers.CSharp;

public class ForbiddenStartTokenCheckerTests
{
    private readonly ForbiddenStartTokenChecker _checker = new();

    [Fact]
    public void Check_WhenLineStartsInsideLiteral_ReturnsNoErrors()
    {
        var line = TestLine.Create(" = B \";");
        line.StartsInsideLiteral = true;

        _checker.Check(line);

        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData(", b", ",")]
    [InlineData("; i++", ";")]
    [InlineData(");", ")")]
    [InlineData("= 1;", "=")]
    [InlineData("+= 1;", "+=")]
    [InlineData("-= 1;", "-=")]
    [InlineData("*= 2;", "*=")]
    [InlineData("/= 2;", "/=")]
    [InlineData("%= 2;", "%=")]
    [InlineData("!= null", "!=")]
    [InlineData(">= 0", ">=")]
    [InlineData("<= 9", "<=")]
    [InlineData("< 9", "<")]
    [InlineData("> 0", ">")]
    // "==" and "=>" are reported as "=" because "=" is checked first
    [InlineData("== 1", "=")]
    [InlineData("=> x * 2;", "=")]
    public void Check_WhenLineStartsWithForbiddenToken_AddsError(string content, string expectedToken)
    {
        var line = TestLine.Create("    " + content, number: 3);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0017), error.Code);
        Assert.Equal(Errors.CH0017.Replace("{token}", expectedToken), error.Message);
        Assert.Equal(3, error.Line);
    }

    [Theory]
    [InlineData("(a, b) => a + b;")]
    [InlineData("() => 1;")]
    [InlineData("(x) => x * 2;")]
    [InlineData("(item)=> item.Id;")]
    [InlineData("(a + b) * c;")]
    [InlineData("(int)value,")]
    [InlineData("(\"US\", ',') => 0x00BC,")]
    public void Check_WhenLineStartsWithParenthesis_AddsNoError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData("var x = 1;")]
    [InlineData("return a + b;")]
    [InlineData("&& b")]
    [InlineData("}")]
    public void Check_WhenLineStartsWithAllowedToken_AddsNoError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }
}
