using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class ForbiddenTokenCheckerTests
{
    private readonly ForbiddenTokenChecker _checker = new();

    [Theory]
    [InlineData("this.Value = 1;")]
    [InlineData("var x = this.Value;")]
    [InlineData("Call(this.Value, this.Other);")]
    public void Check_WhenLineContainsThisDot_AddsOneError(string content)
    {
        var line = TestLine.Create("    " + content, number: 6);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0019), error.Code);
        Assert.Equal(Errors.CH0019.Replace("{token}", "this" + "."), error.Message);
        Assert.Equal(6, error.Line);
    }

    [Theory]
    [InlineData("var thisValue = 1;")]
    [InlineData("return this;")]
    [InlineData("Register(this);")]
    [InlineData("var x = Value;")]
    public void Check_WhenLineDoesNotContainThisDot_AddsNoError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }
}
