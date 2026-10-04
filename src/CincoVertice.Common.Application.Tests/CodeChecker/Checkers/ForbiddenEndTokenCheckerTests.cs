using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class ForbiddenEndTokenCheckerTests
{
    private readonly ForbiddenEndTokenChecker _checker = new();

    [Theory]
    [InlineData("if (a &&", "&&")]
    [InlineData("if (a ||", "||")]
    [InlineData("value ??=", "??=")]
    [InlineData("var x = value ??", "??")]
    [InlineData("var x = flag ?", "?")]
    [InlineData("var x = flag ? a :", ":")]
    [InlineData("var x = a +", "+")]
    [InlineData("var x = a -", "-")]
    [InlineData("var x = a /", "/")]
    [InlineData("var x = a *", "*")]
    [InlineData("var x = !", "!")]
    public void Check_WhenLineEndsWithForbiddenToken_AddsError(string content, string expectedToken)
    {
        var line = TestLine.Create("    " + content, number: 5);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0018), error.Code);
        Assert.Equal(Errors.CH0018.Replace("{token}", expectedToken), error.Message);
        Assert.Equal(5, error.Line);
    }

    [Theory]
    [InlineData("case 1:")]
    [InlineData("case Color.Red:")]
    [InlineData("default:")]
    [InlineData("var name = value!")]
    [InlineData("var x = a + b;")]
    [InlineData("{")]
    [InlineData("}")]
    public void Check_WhenLineEndsWithAllowedToken_AddsNoError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }
}
