using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class TrailingWhitespaceCheckerTests
{
    private readonly TrailingWhitespaceChecker _checker = new();

    [Theory]
    [InlineData("var x = 1; ")]
    [InlineData("var x = 1;\t")]
    [InlineData("var x = 1;   ")]
    public void Check_WhenLineEndsWithWhitespace_AddsError(string content)
    {
        var line = TestLine.Create(content, number: 2);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0011), error.Code);
        Assert.Equal(Errors.CH0011, error.Message);
        Assert.Equal(2, error.Line);
    }

    [Theory]
    [InlineData("var x = 1;")]
    [InlineData("    var x = 1;")]
    [InlineData("\tvar x = 1;")]
    [InlineData("")]
    public void Check_WhenLineDoesNotEndWithWhitespace_AddsNoError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("    ")]
    [InlineData("\t")]
    [InlineData(" \t ")]
    public void Check_WhenLineIsWhitespaceOnly_AddsCh0010AndStops(string content)
    {
        var line = TestLine.Create(content, number: 4);

        var result = _checker.Check(line);

        Assert.False(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0010), error.Code);
        Assert.Equal(Errors.CH0010, error.Message);
        Assert.Equal(4, error.Line);
    }
}
